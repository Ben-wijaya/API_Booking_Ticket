using Exam1.DTO;
using Exam1.Services;
using Microsoft.AspNetCore.Mvc;

namespace Exam1.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class BookTicketController : ControllerBase
    {
        private readonly BookTicketService _bookTicketService;
        private readonly ILogger<BookTicketController> _logger;

        public BookTicketController(BookTicketService bookTicketService, ILogger<BookTicketController> logger)
        {
            _bookTicketService = bookTicketService;
            _logger = logger;
        }

        [HttpPost("book-ticket")]
        public async Task<IActionResult> BookTickets([FromBody] TicketBookingRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            try
            {
                var response = await _bookTicketService.BookTicketsAsync(request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError("Validation error: {Message}", ex.Message);
                return BadRequest(new
                {
                    type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    title = "One or more validation errors occurred.",
                    status = 400,
                    errors = new { message = ex.Message }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");

                return StatusCode(500, new
                {
                    type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                    title = "Internal Server Error",
                    status = 500,
                    errors = new { message = ex.InnerException?.Message ?? ex.Message }
                });
            }
        }

        [HttpGet("get-booked-ticket/{bookedTicketTransactionId}")]
        public async Task<IActionResult> GetBookedTicket(int bookedTicketTransactionId)
        {
            try
            {
                var result = await _bookTicketService.GetBookedTicketAsync(bookedTicketTransactionId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError("Validation error: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("revoke-ticket/{bookedTicketTransactionId}/{ticketCode}/{quantity}")]
        public async Task<IActionResult> RevokeTicket(int bookedTicketTransactionId, string ticketCode, int quantity)
        {
            try
            {
                var result = await _bookTicketService.RevokeTicketAsync(bookedTicketTransactionId, ticketCode, quantity);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError("Validation error: {Message}", ex.Message);
                return BadRequest(new
                {
                    type = "https://tools.ietf.org/html/rfc7807#section-3.1",
                    title = "One or more validation errors occurred.",
                    status = 400,
                    detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return StatusCode(500, new
                {
                    type = "https://tools.ietf.org/html/rfc7807#section-3.2",
                    title = "Internal Server Error",
                    status = 500,
                    detail = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpPut("edit-booked-ticket/{bookedTicketTransactionId}")]
        public async Task<IActionResult> EditBookedTicket(int bookedTicketTransactionId, [FromBody] List<EditBookedTicketRequest> request)
        {
            try
            {
                var result = await _bookTicketService.EditBookedTicketAsync(bookedTicketTransactionId, request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError("Validation error: {Message}", ex.Message);
                return BadRequest(new
                {
                    type = "https://tools.ietf.org/html/rfc7807#section-3.1",
                    title = "One or more validation errors occurred.",
                    status = 400,
                    detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return StatusCode(500, new
                {
                    type = "https://tools.ietf.org/html/rfc7807#section-3.2",
                    title = "Internal Server Error",
                    status = 500,
                    detail = ex.InnerException?.Message ?? ex.Message
                });
            }
        }
    }
}