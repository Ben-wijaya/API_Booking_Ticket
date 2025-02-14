using Exam1.DTO;
using Exam1.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Exam1.Services
{
    public class BookTicketService
    {
        private readonly Exam1Context _db;

        public BookTicketService(Exam1Context db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db), "DbContext is not initialized.");
        }

        public async Task<TicketBookingResponse> BookTicketsAsync(TicketBookingRequest requestDto)
        {
            if (requestDto == null || requestDto.Tickets == null || !requestDto.Tickets.Any())
            {
                throw new ArgumentException("Tickets field cannot be empty.");
            }

            var bookedTickets = new List<BookedTicket>();
            var totalByCategory = new Dictionary<string, decimal>();
            decimal grandTotal = 0;
            int totalTickets = 0;

            foreach (var request in requestDto.Tickets)
            {
                var ticket = await _db.Tickets.Include(t => t.Category)
                    .FirstOrDefaultAsync(t => t.TicketCode == request.TicketCode);

                if (ticket == null)
                {
                    throw new ArgumentException($"Ticket with code '{request.TicketCode}' does not exist.");
                }

                if (ticket.Quota <= 0)
                {
                    throw new ArgumentException($"Ticket '{ticket.TicketName}' is sold out.");
                }

                if (request.Quantity > ticket.Quota)
                {
                    throw new ArgumentException($"Requested quantity for ticket '{ticket.TicketName}' exceeds available quota.");
                }

                // Validasi format tanggal dengan format eksplisit
                if (!DateTime.TryParseExact(request.BookingDate, "yyyy-MM-dd HH:mm:ss.fff",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedBookingDate))
                {
                    throw new ArgumentException($"Invalid date format for ticket '{ticket.TicketName}'. Use 'YYYY-MM-DD HH:mm:ss.fff'.");
                }

                if (parsedBookingDate < ticket.EventDateMinimal || parsedBookingDate > ticket.EventDateMaximal)
                {
                    throw new ArgumentException($"Event date for ticket '{ticket.TicketName}' is not within the valid range.");
                }

                // Kurangi quota tiket
                ticket.Quota -= request.Quantity;
                await _db.SaveChangesAsync();

                // Tambahkan tiket yang dipesan ke daftar transaksi
                var bookedTicket = new BookedTicket
                {
                    TicketCode = ticket.TicketCode,
                    Quantity = request.Quantity,
                    Price = ticket.Price,
                    BookedDate = parsedBookingDate,
                    CreatedAt = DateTime.UtcNow
                };

                bookedTickets.Add(bookedTicket);

                if (!totalByCategory.ContainsKey(ticket.Category.CategoryName))
                {
                    totalByCategory[ticket.Category.CategoryName] = 0;
                }
                totalByCategory[ticket.Category.CategoryName] += bookedTicket.Quantity * bookedTicket.Price;

                grandTotal += bookedTicket.Quantity * bookedTicket.Price;
                totalTickets += bookedTicket.Quantity;
            }

            // Simpan transaksi
            var bookedTransaction = new BookedTicketTransaction
            {
                TotalTickets = totalTickets,
                SummaryPrice = grandTotal,
                CreatedAt = DateTime.UtcNow
            };

            _db.BookedTicketTransactions.Add(bookedTransaction);
            await _db.SaveChangesAsync();

            // Set BookedTicketTransactionId ke semua tiket yang dipesan
            foreach (var bookedTicket in bookedTickets)
            {
                bookedTicket.BookedTicketTransactionId = bookedTransaction.BookedTicketTransactionId;
            }

            _db.BookedTickets.AddRange(bookedTickets);
            await _db.SaveChangesAsync();

            return new TicketBookingResponse
            {
                Tickets = bookedTickets.Select(bt => new BookedTicketDetail
                {
                    TicketCode = bt.TicketCode,
                    TicketName = _db.Tickets
                        .Where(t => t.TicketCode == bt.TicketCode)
                        .Select(t => t.TicketName)
                        .FirstOrDefault() ?? "Unknown",
                    CategoryName = _db.Tickets
                        .Where(t => t.TicketCode == bt.TicketCode)
                        .Select(t => t.Category.CategoryName)
                        .FirstOrDefault() ?? "Unknown",
                    Price = bt.Price,
                    Quantity = bt.Quantity,
                    BookingDate = bt.BookedDate
                }).ToList(),
                CategoryTotals = totalByCategory.Select(ct => new CategorySummary
                {
                    CategoryName = ct.Key,
                    TotalPrice = ct.Value
                }).ToList(),
                GrandTotal = grandTotal,
                TotalTickets = totalTickets
            };
        }

        public async Task<List<GetBookedTicketResponse>> GetBookedTicketAsync(int bookedTicketTransactionId)
        {
            var bookedTickets = await _db.BookedTickets
                .Include(bt => bt.TicketCodeNavigation)
                .ThenInclude(t => t.Category)
                .Where(bt => bt.BookedTicketTransactionId == bookedTicketTransactionId)
                .ToListAsync();

            if (!bookedTickets.Any())
            {
                throw new ArgumentException("BookedTicketTransactionId not found.");
            }

            // Kelompokkan tiket berdasarkan kategori
            var groupedTickets = bookedTickets
                .GroupBy(bt => bt.TicketCodeNavigation.Category.CategoryName)
                .Select(g => new GetBookedTicketResponse
                {
                    CategoryName = g.Key,
                    QtyPerCategory = g.Sum(bt => bt.Quantity),
                    Tickets = g.Select(bt => new GetBookedTicketInfo
                    {
                        TicketCode = bt.TicketCode,
                        TicketName = bt.TicketCodeNavigation.TicketName,
                        BookingDate = bt.BookedDate,
                        Quantity = bt.Quantity
                    }).ToList()
                }).ToList();

            return groupedTickets;
        }

        public async Task<RevokeTicketResponse> RevokeTicketAsync(int bookedTicketTransactionId, string ticketCode, int quantity)
        {
            // Validasi BookedTicketTransactionId
            var bookedTickets = await _db.BookedTickets
                .Include(bt => bt.TicketCodeNavigation)
                .ThenInclude(t => t.Category)
                .Where(bt => bt.BookedTicketTransactionId == bookedTicketTransactionId)
                .ToListAsync();

            if (!bookedTickets.Any())
            {
                throw new ArgumentException("BookedTicketTransactionId not found.");
            }

            // Validasi Kode Tiket
            var bookedTicket = bookedTickets.FirstOrDefault(bt => bt.TicketCode == ticketCode);
            if (bookedTicket == null)
            {
                throw new ArgumentException($"Ticket with code '{ticketCode}' not found in the booked tickets.");
            }

            // Validasi Quantity
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.");
            }

            if (quantity > bookedTicket.Quantity)
            {
                throw new ArgumentException($"Requested quantity ({quantity}) exceeds the booked quantity ({bookedTicket.Quantity}).");
            }

            // Update Quantity
            bookedTicket.Quantity -= quantity;

            // Update TotalTickets pada BookedTicketTransaction
            var transaction = await _db.BookedTicketTransactions
                .FirstOrDefaultAsync(t => t.BookedTicketTransactionId == bookedTicketTransactionId);
            if (transaction != null)
            {
                transaction.TotalTickets -= quantity; // Kurangi TotalTickets sesuai quantity yang di-revoke
            }

            // Jika quantity menjadi 0, hapus row tersebut
            if (bookedTicket.Quantity == 0)
            {
                _db.BookedTickets.Remove(bookedTicket);
            }

            // Jika semua tiket pada BookedTicketTransactionId sudah dihapus, hapus seluruh row
            if (!bookedTickets.Any(bt => bt.Quantity > 0))
            {
                if (transaction != null)
                {
                    _db.BookedTicketTransactions.Remove(transaction);
                }
            }

            await _db.SaveChangesAsync();

            return new RevokeTicketResponse
            {
                TicketCode = bookedTicket.TicketCode,
                TicketName = bookedTicket.TicketCodeNavigation.TicketName,
                CategoryName = bookedTicket.TicketCodeNavigation.Category.CategoryName,
                RemainingQuantity = bookedTicket.Quantity
            };
        }

        public async Task<List<EditBookedTicketResponse>> EditBookedTicketAsync(int bookedTicketTransactionId, List<EditBookedTicketRequest> request)
        {
            // Validasi BookedTicketTransactionId
            var bookedTickets = await _db.BookedTickets
                .Include(bt => bt.TicketCodeNavigation)
                .ThenInclude(t => t.Category)
                .Where(bt => bt.BookedTicketTransactionId == bookedTicketTransactionId)
                .ToListAsync();

            if (!bookedTickets.Any())
            {
                throw new ArgumentException("BookedTicketTransactionId not found.");
            }

            var response = new List<EditBookedTicketResponse>();
            var transaction = await _db.BookedTicketTransactions
                .FirstOrDefaultAsync(t => t.BookedTicketTransactionId == bookedTicketTransactionId);

            if (transaction == null)
            {
                throw new ArgumentException("BookedTicketTransaction not found.");
            }

            foreach (var item in request)
            {
                // Validasi Kode Tiket
                var bookedTicket = bookedTickets.FirstOrDefault(bt => bt.TicketCode == item.TicketCode);
                if (bookedTicket == null)
                {
                    throw new ArgumentException($"Ticket with code '{item.TicketCode}' not found in the booked tickets.");
                }

                // Validasi Quantity
                if (item.Quantity <= 0)
                {
                    throw new ArgumentException($"Quantity for ticket '{item.TicketCode}' must be greater than zero.");
                }

                var ticket = await _db.Tickets
                    .FirstOrDefaultAsync(t => t.TicketCode == item.TicketCode);

                if (ticket == null)
                {
                    throw new ArgumentException($"Ticket with code '{item.TicketCode}' not found in the tickets table.");
                }

                if (item.Quantity > ticket.Quota + bookedTicket.Quantity)
                {
                    throw new ArgumentException($"Requested quantity ({item.Quantity}) for ticket '{item.TicketCode}' exceeds available quota.");
                }

                // Update quota tiket
                ticket.Quota += bookedTicket.Quantity - item.Quantity;
                await _db.SaveChangesAsync();

                // Update quantity pada BookedTickets
                var oldQuantity = bookedTicket.Quantity;
                bookedTicket.Quantity = item.Quantity;

                // Update TotalTickets pada BookedTicketTransaction
                transaction.TotalTickets += item.Quantity - oldQuantity;

                // Tambahkan ke respons
                response.Add(new EditBookedTicketResponse
                {
                    TicketCode = bookedTicket.TicketCode,
                    TicketName = bookedTicket.TicketCodeNavigation.TicketName,
                    CategoryName = bookedTicket.TicketCodeNavigation.Category.CategoryName,
                    RemainingQuantity = bookedTicket.Quantity
                });
            }

            await _db.SaveChangesAsync();

            return response;
        }
    }
}