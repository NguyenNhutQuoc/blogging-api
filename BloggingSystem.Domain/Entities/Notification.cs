using System;
using System.Collections.Generic;

namespace BloggingSystem.Domain.Entities;

public partial class Notification
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long? SenderId { get; set; }

    public string Type { get; set; } = null!;

    public string EntityType { get; set; } = null!;

    public long EntityId { get; set; }

    public string Message { get; set; } = null!;

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? Sender { get; set; }

    public virtual User User { get; set; } = null!;
    
    private Notification() {}
}
