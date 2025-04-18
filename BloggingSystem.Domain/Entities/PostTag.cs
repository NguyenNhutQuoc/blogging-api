using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BloggingSystem.Domain.Commons;

namespace BloggingSystem.Domain.Entities;

public partial class PostTag: BaseEntity
{
    
    [NotMapped]
    public DateTime UpdatedAt { get; set; }
    public long Id { get; set; }

    public long PostId { get; set; }

    public long TagId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual Tag Tag { get; set; } = null!;
    
    private PostTag() {}
    
    public PostTag(long postId, long tagId)
    {
        PostId = postId;
        TagId = tagId;
    }
    
    public static PostTag Create(long postId, long tagId)
    {
        return new PostTag(postId, tagId);
    }
    
    public void Update(long tagId)
    {
        TagId = tagId;
    }
}
