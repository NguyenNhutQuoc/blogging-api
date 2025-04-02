using System;
using System.Collections.Generic;

namespace BloggingSystem.Domain.Entities;

public partial class Newsletter
{
    public long Id { get; set; }

    public string Subject { get; set; } = null!;

    public string Content { get; set; } = null!;

    public long SentBy { get; set; }

    public string? Status { get; set; }

    public DateTime? SentAt { get; set; }

    public DateTime? ScheduledAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<NewsletterStat> NewsletterStats { get; set; } = new List<NewsletterStat>();

    public virtual User SentByNavigation { get; set; } = null!;
    
    private Newsletter() {}
}

public enum NewsletterStatus
{
    Draft,
    Scheduled,
    Sent,
    Cancelled,
}