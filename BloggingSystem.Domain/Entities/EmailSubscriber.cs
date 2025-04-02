using System;
using System.Collections.Generic;

namespace BloggingSystem.Domain.Entities;

public partial class EmailSubscriber
{
    public long Id { get; set; }

    public string Email { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Status { get; set; }

    public string? Token { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
    
    private EmailSubscriber() {}
}

public enum EmailSubscriberStatus
{
    Subscribed,
    Unsubscribed,
    Pending
}
