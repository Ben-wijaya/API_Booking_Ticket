using Exam1.Models;
using Exam1.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Exam1.Controllers
{
    [Route("api/v1/reports")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly PdfReportService _pdfReportService;
        private readonly TicketService _ticketService;

        public ReportController(PdfReportService pdfReportService, TicketService ticketService)
        {
            _pdfReportService = pdfReportService;
            _ticketService = ticketService;
        }

        [HttpGet("download-ticket-report")]
        public async Task<IActionResult> DownloadTicketReport()
        {
            try
            {
                // Ambil data tiket yang masih available
                var (availableTickets, _) = await _ticketService.GetAvailableTickets(
                    categoryName: null,
                    ticketCode: null,
                    ticketName: null,
                    maxPrice: null,
                    eventDateMin: null,
                    eventDateMax: null,
                    orderBy: "TicketCode",
                    orderState: "asc",
                    page: 1,
                    pageSize: int.MaxValue); // Ambil semua tiket yang available

                // Ambil data tiket yang sudah dibooked
                var bookedTickets = await _ticketService.GetBookedTicketsAsync();

                // Generate PDF report
                var pdfBytes = _pdfReportService.GenerateTicketReport(availableTickets, bookedTickets);

                // Kembalikan file PDF
                return File(pdfBytes, "application/pdf", "TicketReport.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807#section-3.2",
                    Title = "Internal server error.",
                    Status = 500,
                    Detail = ex.Message
                });
            }
        }
    }
}