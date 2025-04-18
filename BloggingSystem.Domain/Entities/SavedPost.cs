using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BloggingSystem.Domain.Commons;
using BloggingSystem.Domain.Events;

namespace BloggingSystem.Domain.Entities;

public partial class SavedPost: BaseEntity
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long PostId { get; set; }

    [NotMapped]
    public DateTime? UpdatedAt { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
    
    private SavedPost() {}
    
    public SavedPost(long userId, long postId)
    {
        UserId = userId;
        PostId = postId;
    }
    
    public static SavedPost Create(long userId, long postId)
    {
        var savedPost = new SavedPost(userId, postId);

        savedPost.AddDomainEvent(new PostSavedEvent(
                userId,
                postId));

        return savedPost;
    }

    public void Delete(long userId, long  postId) {
        AddDomainEvent(new PostUnsavedEvent(userId, postId));
    }
}
