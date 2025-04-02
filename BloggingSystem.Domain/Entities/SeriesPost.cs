using System;
using System.Collections.Generic;

namespace BloggingSystem.Domain.Entities;

public partial class SeriesPost
{
    public long Id { get; set; }

    public long SeriesId { get; set; }

    public long PostId { get; set; }

    public int Position { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual Series Series { get; set; } = null!;
    
    private SeriesPost() {}
}
