using System;
using System.Collections.Generic;

namespace BloggingSystem.Domain.Entities;

public partial class PostMedium
{
    public long Id { get; set; }

    public long PostId { get; set; }

    public long MediaId { get; set; }

    public int? SortOrder { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Medium Media { get; set; } = null!;

    public virtual Post Post { get; set; } = null!;
    
    private PostMedium() {}
}
