using System;
using System.Collections.Generic;
using BloggingSystem.Domain.Commons;

namespace BloggingSystem.Domain.Entities;

public partial class Category:BaseEntity
{
    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public long? ParentId { get; set; }

    public virtual ICollection<Category> InverseParent { get; set; } = new List<Category>();

    public virtual Category? Parent { get; set; }

    public virtual ICollection<PostCategory> PostCategories { get; set; } = new List<PostCategory>();

    private Category()
    {
        
    }
    
    public Category(string name, string slug, string? description = null)
    {
        Name = name;
        Slug = slug;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }
    
    public static Category Create(string name, string slug, string? description = null)
    {
        return new Category(name, slug, description);
    }
    
    public void Update(string name, string slug, string? description = null)
    {
        Name = name;
        Slug = slug;
        Description = description;
        
        SetModified();
    }
    
}