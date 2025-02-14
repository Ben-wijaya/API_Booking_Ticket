using Exam1.DTO;
using Exam1.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exam1.Services
{
    public class TicketService
    {
        private readonly Exam1Context _db;

        public TicketService(Exam1Context db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db), "DbContext is not initialized.");
        }

        public async Task<(List<TicketOutputDto> Tickets, int TotalCount)> GetAvailableTickets(
    string? categoryName, string? ticketCode, string? ticketName, decimal? maxPrice,
    DateTime? eventDateMin, DateTime? eventDateMax,
    string? orderBy = "TicketCode", string? orderState = "asc",
    int page = 1, int pageSize = 10)
        {
            if (_db.Tickets == null || !_db.Tickets.Any())
            {
                throw new ArgumentException("No tickets available in the database.");
            }

            // Query jika semua parameter null / tidak ada input yg diberikan
            var query = _db.Tickets
                .Include(t => t.Category)
                .Where(t => t.Quota > 0);

            // Filter hanya jika parameter tidak null
            if (!string.IsNullOrEmpty(categoryName))
            {
                query = query.Where(t => t.Category != null && t.Category.CategoryName.Contains(categoryName));
            }

            if (!string.IsNullOrEmpty(ticketCode))
            {
                query = query.Where(t => t.TicketCode != null && t.TicketCode.Contains(ticketCode));
            }

            if (!string.IsNullOrEmpty(ticketName))
            {
                query = query.Where(t => t.TicketName != null && t.TicketName.Contains(ticketName));
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(t => t.Price <= maxPrice);
            }

            if (eventDateMin.HasValue)
            {
                query = query.Where(t => t.EventDateMinimal >= eventDateMin);
            }

            if (eventDateMax.HasValue)
            {
                query = query.Where(t => t.EventDateMaximal <= eventDateMax);
            }

            // Validasi orderState
            orderState = orderState?.ToLower();
            if (orderState != "asc" && orderState != "desc")
            {
                orderState = "asc";
            }

            // Validasi orderBy
            var validOrderColumns = new HashSet<string> { "categoryname", "ticketcode", "ticketname", "price", "eventdateminimal", "eventdatemaximal" };
            if (!string.IsNullOrEmpty(orderBy))
            {
                string orderByLower = orderBy.ToLower();

                if (!validOrderColumns.Contains(orderByLower))
                {
                    throw new ArgumentException($"Invalid orderBy value '{orderBy}'. Available options: {string.Join(", ", validOrderColumns)}");
                }

                orderBy = orderByLower;
            }

            bool isDescending = orderState == "desc";

            query = orderBy switch
            {
                "categoryname" => isDescending
                    ? query.OrderByDescending(t => t.Category != null ? t.Category.CategoryName : string.Empty)
                    : query.OrderBy(t => t.Category != null ? t.Category.CategoryName : string.Empty),

                "ticketcode" => isDescending
                    ? query.OrderByDescending(t => t.TicketCode)
                    : query.OrderBy(t => t.TicketCode),

                "ticketname" => isDescending
                    ? query.OrderByDescending(t => t.TicketName)
                    : query.OrderBy(t => t.TicketName),

                "price" => isDescending
                    ? query.OrderByDescending(t => t.Price)
                    : query.OrderBy(t => t.Price),

                "eventdateminimal" => isDescending
                    ? query.OrderByDescending(t => t.EventDateMinimal)
                    : query.OrderBy(t => t.EventDateMinimal),

                "eventdatemaximal" => isDescending
                    ? query.OrderByDescending(t => t.EventDateMaximal)
                    : query.OrderBy(t => t.EventDateMaximal),

                // Default sort
                _ => isDescending
                    ? query.OrderByDescending(t => t.TicketCode)
                    : query.OrderBy(t => t.TicketCode)
            };

            // Hitung total jumlah data (untuk pagination)
            int totalCount = await query.CountAsync();

            // Terapkan pagination
            var tickets = await query
                .Skip((page - 1) * pageSize) // Lewati data sebelumnya
                .Take(pageSize) // Ambil data sebanyak pageSize
                .ToListAsync();

            if (tickets == null || tickets.Count == 0)
            {
                throw new ArgumentException("No tickets match the specified criteria.");
            }

            // Map ke DTO
            var ticketDtos = tickets.Select(t => new TicketOutputDto
            {
                TicketCode = t.TicketCode ?? "N/A",
                TicketName = t.TicketName ?? "N/A",
                CategoryName = t.Category?.CategoryName ?? "N/A",
                Price = t.Price,
                Quota = t.Quota,
                EventDateMinimal = t.EventDateMinimal.ToString("yyyy-MM-dd HH:mm:ss"),
                EventDateMaximal = t.EventDateMaximal.ToString("yyyy-MM-dd HH:mm:ss")
            }).ToList();

            return (ticketDtos, totalCount);
        }

        public async Task<List<BookedTicketDetail>> GetBookedTicketsAsync()
        {
            var bookedTickets = await _db.BookedTickets
                .Include(bt => bt.TicketCodeNavigation)
                .ThenInclude(t => t.Category)
                .Select(bt => new BookedTicketDetail
                {
                    TicketCode = bt.TicketCode,
                    TicketName = bt.TicketCodeNavigation.TicketName,
                    CategoryName = bt.TicketCodeNavigation.Category.CategoryName,
                    Quantity = bt.Quantity,
                    BookingDate = bt.BookedDate
                })
                .ToListAsync();

            return bookedTickets;
        }
    }
}