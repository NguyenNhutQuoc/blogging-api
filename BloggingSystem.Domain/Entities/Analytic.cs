using System;
using System.Collections.Generic;

namespace BloggingSystem.Domain.Entities;

public partial class Analytic
{
    public long Id { get; set; }

    public long PostId { get; set; }

    public int? Views { get; set; }

    public int? UniqueVisitors { get; set; }

    public int? AvgTimeOnPage { get; set; }

    public decimal? BounceRate { get; set; }

    public DateOnly Date { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Post Post { get; set; } = null!;
    
    private Analytic() {}
}
