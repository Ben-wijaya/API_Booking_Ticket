using Exam1.DTO;
using Exam1.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Globalization;

namespace Exam1.Services
{
    public class PdfReportService
    {
        public byte[] GenerateTicketReport(List<TicketOutputDto> availableTickets, List<BookedTicketDetail> bookedTickets)
        {
            // Set license (gratis untuk proyek non-komersial)
            QuestPDF.Settings.License = LicenseType.Community;

            // Buat dokumen PDF
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text("Ticket Report")
                        .SemiBold().FontSize(24).AlignCenter();

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            // Section 1: Available Tickets
                            column.Item().Text("Available Tickets").Bold().FontSize(18);
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(); // Ticket Code
                                    columns.RelativeColumn(); // Ticket Name
                                    columns.RelativeColumn(); // Category Name
                                    columns.RelativeColumn(); // Price
                                    columns.RelativeColumn(); // Quota
                                    columns.RelativeColumn(); // Event Date Minimal
                                    columns.RelativeColumn(); // Event Date Maximal
                                });

                                // Header tabel
                                table.Header(header =>
                                {
                                    header.Cell().Text("Ticket Code").Bold();
                                    header.Cell().Text("Ticket Name").Bold();
                                    header.Cell().Text("Category Name").Bold();
                                    header.Cell().Text("Price").Bold();
                                    header.Cell().Text("Quota").Bold();
                                    header.Cell().Text("Event Date Min").Bold();
                                    header.Cell().Text("Event Date Max").Bold();
                                });

                                // Isi tabel (Available Tickets)
                                foreach (var ticket in availableTickets)
                                {
                                    table.Cell().Text(ticket.TicketCode);
                                    table.Cell().Text(ticket.TicketName);
                                    table.Cell().Text(ticket.CategoryName);
                                    table.Cell().Text(FormatRupiah(ticket.Price));
                                    table.Cell().Text(ticket.Quota?.ToString() ?? "0");
                                    table.Cell().Text(ticket.EventDateMinimal);
                                    table.Cell().Text(ticket.EventDateMaximal);
                                }
                            });

                            // Section 2: Booked Tickets
                            column.Item().PaddingTop(20).Text("Booked Tickets").Bold().FontSize(18);
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(); // Ticket Code
                                    columns.RelativeColumn(); // Ticket Name
                                    columns.RelativeColumn(); // Category Name
                                    columns.RelativeColumn(); // Quantity
                                    columns.RelativeColumn(); // Booking Date
                                });

                                // Header tabel
                                table.Header(header =>
                                {
                                    header.Cell().Text("Ticket Code").Bold();
                                    header.Cell().Text("Ticket Name").Bold();
                                    header.Cell().Text("Category Name").Bold();
                                    header.Cell().Text("Quantity").Bold();
                                    header.Cell().Text("Booking Date").Bold();
                                });

                                // Isi tabel (Booked Tickets)
                                foreach (var ticket in bookedTickets)
                                {
                                    table.Cell().Text(ticket.TicketCode);
                                    table.Cell().Text(ticket.TicketName);
                                    table.Cell().Text(ticket.CategoryName);
                                    table.Cell().Text(ticket.Quantity.ToString());
                                    table.Cell().Text(ticket.BookingDate.ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                            });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            });

            // Generate PDF sebagai byte array
            return document.GeneratePdf();
        }

        // memformat price ke Rupiah
        private string FormatRupiah(decimal amount)
        {
            return $"Rp.{amount.ToString("N0", CultureInfo.CreateSpecificCulture("id-ID"))}";
        }
    }
}