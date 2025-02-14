using System;
using System.Collections.Generic;

namespace Exam1.Entities;

public partial class BookedTicket
{
    public int BookedTicketId { get; set; }

    public int BookedTicketTransactionId { get; set; }

    public string TicketCode { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public DateTime BookedDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual BookedTicketTransaction BookedTicketTransaction { get; set; } = null!;

    public virtual Ticket TicketCodeNavigation { get; set; } = null!;
}
