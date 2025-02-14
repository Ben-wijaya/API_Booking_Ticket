namespace Exam1.Models
{
    public class TicketReportModel
    {
        public string TicketCode { get; set; }
        public string TicketName { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public int Quota { get; set; }
        public string EventDateMinimal { get; set; }
        public string EventDateMaximal { get; set; }
    }
}