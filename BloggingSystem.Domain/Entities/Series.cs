using System;
using System.Collections.Generic;

namespace BloggingSystem.Domain.Entities;

public partial class Series
{
    public long Id { get; set; }

    public long AuthorId { get; set; }

    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual ICollection<SeriesPost> SeriesPosts { get; set; } = new List<SeriesPost>();
    
    private Series() {}
}

public enum SeriesStatus
{
    Draft,
    Published,
    Archived
}