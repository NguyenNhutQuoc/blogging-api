using System;
using System.Collections.Generic;

namespace BloggingSystem.Domain.Entities;

public partial class SeoMetadatum
{
    public long Id { get; set; }

    public string EntityType { get; set; } = null!;

    public long EntityId { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public string? MetaKeywords { get; set; }

    public string? OgTitle { get; set; }

    public string? OgDescription { get; set; }

    public string? OgImageUrl { get; set; }

    public string? TwitterCard { get; set; }

    public string? TwitterTitle { get; set; }

    public string? TwitterDescription { get; set; }

    public string? TwitterImageUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
    
    private SeoMetadatum() {}
}

public enum SeoMetadatumEntityType
{
    Post,
    Category,
    Tag,
    User
}