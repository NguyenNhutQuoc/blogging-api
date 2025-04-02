using System;
using System.Collections.Generic;

namespace BloggingSystem.Domain.Entities;

public partial class PollOption
{
    public long Id { get; set; }

    public long PollId { get; set; }

    public string OptionText { get; set; } = null!;

    public int? SortOrder { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Poll Poll { get; set; } = null!;

    public virtual ICollection<PollVote> PollVotes { get; set; } = new List<PollVote>();
    
    private PollOption() {}
}
