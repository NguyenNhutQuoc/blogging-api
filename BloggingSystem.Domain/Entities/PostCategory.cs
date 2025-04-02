using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BloggingSystem.Domain.Commons;

namespace BloggingSystem.Domain.Entities;

public partial class PostCategory: BaseEntity
{
    [NotMapped]
    public DateTime UpdatedAt { get; set; } 
    public long PostId { get; set; }

    public long CategoryId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Post Post { get; set; } = null!;
    
    private PostCategory() {}
    
    public PostCategory(long postId, long categoryId)
    {
        PostId = postId;
        CategoryId = categoryId;
    }
    
    public static PostCategory Create(long postId, long categoryId)
    {
        return new PostCategory(postId, categoryId);
    }
}
