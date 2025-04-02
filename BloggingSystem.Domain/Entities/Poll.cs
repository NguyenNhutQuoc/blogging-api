using System;
using System.Collections.Generic;

namespace BloggingSystem.Domain.Entities;

public partial class Poll
{
    public long Id { get; set; }

    public long CreatorId { get; set; }

    public long? PostId { get; set; }

    public string Question { get; set; } = null!;

    public string? Status { get; set; }

    public bool? AllowMultiple { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User Creator { get; set; } = null!;

    public virtual ICollection<PollOption> PollOptions { get; set; } = new List<PollOption>();

    public virtual ICollection<PollVote> PollVotes { get; set; } = new List<PollVote>();

    public virtual Post? Post { get; set; }
    
    private Poll() {}
}

public enum PollStatus
{
    Active,
    Closed
}