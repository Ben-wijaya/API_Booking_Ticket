using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Exam1.DTO
{
    public class TicketBookingRequest
    {
        [Required(ErrorMessage = "Tickets field is required.")]
        [JsonPropertyName("tickets")]
        public List<TicketBookingDetail> Tickets { get; set; }
    }

    public class TicketBookingDetail
    {
        [Required(ErrorMessage = "TicketCode is required.")]
        [JsonPropertyName("ticketCode")]
        public string TicketCode { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "BookingDate is required.")]
        [JsonPropertyName("bookingDate")]
        public string BookingDate { get; set; }
    }

    public class TicketBookingResponse
    {
        [JsonPropertyName("tickets")]
        public List<BookedTicketDetail> Tickets { get; set; }

        [JsonPropertyName("categoryTotals")]
        public List<CategorySummary> CategoryTotals { get; set; }

        [JsonPropertyName("grandTotal")]
        public decimal GrandTotal { get; set; }

        [JsonPropertyName("totalTickets")]
        public int TotalTickets { get; set; }
    }

    public class BookedTicketDetail
    {
        [JsonPropertyName("ticketCode")]
        public string TicketCode { get; set; }

        [JsonPropertyName("ticketName")]
        public string TicketName { get; set; }

        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("bookingDate")]
        public DateTime BookingDate { get; set; }
    }

    public class CategorySummary
    {
        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; }

        [JsonPropertyName("totalPrice")]
        public decimal TotalPrice { get; set; }
    }

    // Class baru untuk respons get-booked-ticket
    public class GetBookedTicketResponse
    {
        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; }

        [JsonPropertyName("qtyPerCategory")]
        public int QtyPerCategory { get; set; }

        [JsonPropertyName("tickets")]
        public List<GetBookedTicketInfo> Tickets { get; set; }
    }

    public class GetBookedTicketInfo
    {
        [JsonPropertyName("ticketCode")]
        public string TicketCode { get; set; }

        [JsonPropertyName("ticketName")]
        public string TicketName { get; set; }

        [JsonPropertyName("bookingDate")]
        public DateTime BookingDate { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }

    public class RevokeTicketResponse
    {
        [JsonPropertyName("ticketCode")]
        public string TicketCode { get; set; }

        [JsonPropertyName("ticketName")]
        public string TicketName { get; set; }

        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; }

        [JsonPropertyName("remainingQuantity")]
        public int RemainingQuantity { get; set; }
    }

    public class EditBookedTicketRequest
    {
        [Required(ErrorMessage = "TicketCode is required.")]
        [JsonPropertyName("ticketCode")]
        public string TicketCode { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }

    public class EditBookedTicketResponse
    {
        [JsonPropertyName("ticketCode")]
        public string TicketCode { get; set; }

        [JsonPropertyName("ticketName")]
        public string TicketName { get; set; }

        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; }

        [JsonPropertyName("remainingQuantity")]
        public int RemainingQuantity { get; set; }
    }
}