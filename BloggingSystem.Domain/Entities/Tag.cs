using System;
using System.Collections.Generic;
using BloggingSystem.Domain.Commons;

namespace BloggingSystem.Domain.Entities;

public partial class Tag: BaseEntity
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    
    private Tag() {}
    
    public Tag(string name, string slug)
    {
        Name = name;
        Slug = slug;
        CreatedAt = DateTime.UtcNow;
    }
    
    public static Tag Create(string name, string slug)
    {
        return new Tag(name, slug);
    }
    
    public void Update(string name, string slug)
    {
        Name = name;
        Slug = slug;
    }
    
    
}
