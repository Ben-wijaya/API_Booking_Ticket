namespace Exam1.DTO
{
    public class TicketOutputDto
    {
        public string TicketCode { get; set; }
        public string TicketName { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public int? Quota { get; set; }
        public string EventDateMinimal { get; set; } // Format: "yyyy-MM-dd HH:mm:ss"
        public string EventDateMaximal { get; set; } // Format: "yyyy-MM-dd HH:mm:ss"
    }
}
