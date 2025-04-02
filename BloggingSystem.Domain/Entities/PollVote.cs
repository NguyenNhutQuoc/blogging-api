using System;
using System.Collections.Generic;

namespace BloggingSystem.Domain.Entities;

public partial class PollVote
{
    public long Id { get; set; }

    public long PollId { get; set; }

    public long OptionId { get; set; }

    public long? UserId { get; set; }

    public string IpAddress { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual PollOption Option { get; set; } = null!;

    public virtual Poll Poll { get; set; } = null!;

    public virtual User? User { get; set; }
    
    private PollVote() {}
}
