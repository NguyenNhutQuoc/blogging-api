using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BloggingSystem.Domain.Commons;

namespace BloggingSystem.Domain.Entities;

public partial class PostMedium: BaseEntity
{
    public long PostId { get; set; }

    public long MediaId { get; set; }

    public int? SortOrder { get; set; }

    [NotMapped]
    public DateTime? UpdatedAt { get; set; }

    public virtual Medium Media { get; set; } = null!;

    public virtual Post Post { get; set; } = null!;
    
    private PostMedium() {}
    
    public PostMedium(long postId, long mediaId, int? sortOrder)
    {
        PostId = postId;
        MediaId = mediaId;
        SortOrder = sortOrder;
    }
    
    public PostMedium(long postId, long mediaId)
    {
        PostId = postId;
        MediaId = mediaId;
    }
    
    public static PostMedium Create(long postId, long mediaId, int? sortOrder)
    {
        return new PostMedium(postId, mediaId, sortOrder);
    }
}
