using System;
using System.Collections.Generic;

namespace Exam1.Entities;

public partial class Ticket
{
    public string TicketCode { get; set; } = null!;

    public string TicketName { get; set; }

    public int CategoryId { get; set; }

    public decimal Price { get; set; }

    public int? Quota { get; set; }

    public DateTime EventDateMinimal { get; set; }

    public DateTime EventDateMaximal { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<BookedTicket> BookedTickets { get; set; } = new List<BookedTicket>();

    public virtual Category Category { get; set; } = null!;
}
