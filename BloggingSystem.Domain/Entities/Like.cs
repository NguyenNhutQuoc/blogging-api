using System;
using System.Collections.Generic;

namespace BloggingSystem.Domain.Entities;

public partial class Like
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string EntityType { get; set; } = null!;

    public long EntityId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
    
    private Like() {}
}

public enum LikeEntityType
{
    Post,
    Comment
}
