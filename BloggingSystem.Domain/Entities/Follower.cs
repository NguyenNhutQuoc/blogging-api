using System;
using System.Collections.Generic;

namespace BloggingSystem.Domain.Entities;

public partial class Follower
{
    public long Id { get; set; }

    public long FollowerId { get; set; }

    public long FollowingId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User FollowerNavigation { get; set; } = null!;

    public virtual User Following { get; set; } = null!;
    
    private Follower() {}
}
