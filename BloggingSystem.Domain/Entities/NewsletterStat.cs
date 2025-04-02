using System;
using System.Collections.Generic;

namespace BloggingSystem.Domain.Entities;

public partial class NewsletterStat
{
    public long Id { get; set; }

    public long NewsletterId { get; set; }

    public int? TotalSent { get; set; }

    public int? TotalDelivered { get; set; }

    public int? TotalOpened { get; set; }

    public int? TotalClicked { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Newsletter Newsletter { get; set; } = null!;
    
    private NewsletterStat() {}
}
