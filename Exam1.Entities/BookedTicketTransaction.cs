using System;
using System.Collections.Generic;

namespace Exam1.Entities;

public partial class BookedTicketTransaction
{
    public int BookedTicketTransactionId { get; set; }

    public int TotalTickets { get; set; }

    public decimal SummaryPrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<BookedTicket> BookedTickets { get; set; } = new List<BookedTicket>();
}
