using Exam1.Services;
using Microsoft.AspNetCore.Mvc;

namespace Exam1.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class AvailableTicketController : ControllerBase
    {
        private readonly TicketService _ticketService;

        public AvailableTicketController(TicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet("get-available-ticket")]
        public async Task<IActionResult> GetAvailableTickets(
    [FromQuery] string? categoryName,
    [FromQuery] string? ticketCode,
    [FromQuery] string? ticketName,
    [FromQuery] decimal? maxPrice,
    [FromQuery] DateTime? eventDateMin,
    [FromQuery] DateTime? eventDateMax,
    [FromQuery] string? orderBy,
    [FromQuery] string? orderState,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Type = "https://tools.ietf.org/html/rfc7807#section-3.1",
                        Title = "Invalid input data.",
                        Status = 400,
                        Detail = "One or more input values are invalid."
                    });
                }

                // Validasi page dan pageSize
                if (page < 1)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Type = "https://tools.ietf.org/html/rfc7807#section-3.1",
                        Title = "Invalid page number.",
                        Status = 400,
                        Detail = "Page number must be greater than or equal to 1."
                    });
                }

                if (pageSize < 1)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Type = "https://tools.ietf.org/html/rfc7807#section-3.1",
                        Title = "Invalid page size.",
                        Status = 400,
                        Detail = "Page size must be greater than or equal to 1."
                    });
                }

                var (tickets, totalCount) = await _ticketService.GetAvailableTickets(
                    categoryName, ticketCode, ticketName, maxPrice, eventDateMin, eventDateMax, orderBy, orderState, page, pageSize);

                if (tickets == null || tickets.Count == 0)
                {
                    return NotFound(new ProblemDetails
                    {
                        Type = "https://tools.ietf.org/html/rfc7807#section-3.4",
                        Title = "No available tickets found.",
                        Status = 404,
                        Detail = "No tickets match the specified criteria."
                    });
                }

                // Hitung total halaman
                int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                // Kembalikan respons dengan informasi pagination
                return Ok(new
                {
                    Data = tickets,
                    Pagination = new
                    {
                        TotalCount = totalCount,
                        TotalPages = totalPages,
                        CurrentPage = page,
                        PageSize = pageSize
                    }
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807#section-3.1",
                    Title = "Invalid request.",
                    Status = 400,
                    Detail = ex.Message
                });
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