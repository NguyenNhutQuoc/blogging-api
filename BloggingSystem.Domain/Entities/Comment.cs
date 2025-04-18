using System;
using System.Collections.Generic;
using BloggingSystem.Domain.Commons;
using BloggingSystem.Domain.Events;

namespace BloggingSystem.Domain.Entities;

public partial class Comment: BaseEntity
{

    public long PostId { get; set; }

    public long UserId { get; set; }

    public long? ParentId { get; set; }

    public string Content { get; set; } = null!;

    public string? Status { get; set; }

    public virtual ICollection<Comment> InverseParent { get; set; } = new List<Comment>();

    public virtual Comment? Parent { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
    
    private Comment() {}
    
    public Comment(long postId, long userId, string content)
    {
        PostId = postId;
        UserId = userId;
        Content = content;
        CreatedAt = DateTime.UtcNow;
        Status = CommentStatus.Pending.ToString();
    }
    
    public static Comment Create(long postId, long userId, string content)
    {
        var comment  = new Comment(postId, userId, content);

        comment.AddDomainEvent(new CreatedCommentEvent(userId, postId, content));

        return comment;
    }
    
    public void Update(string content)
    {
        Content = content;
        
        AddDomainEvent(new UpdatedCommentEvent(UserId, PostId, Content, Status));
        SetModified();
    }
    
    public void UpdateStatus(CommentStatus status)
    {
        Status = status.ToString();
        AddDomainEvent(new UpdatedCommentEvent(UserId, PostId, Content, Status));
        SetModified();
    }
    
    public void SetParent(Comment parent)
    {
        Parent = parent;
        ParentId = parent.Id;
        
        SetModified();
    }
    
    public void RemoveParent()
    {
        Parent = null;
        ParentId = null;
        
        SetModified();
    }
    
    public void Trash()
    {
        Status = CommentStatus.Trash.ToString();
        AddDomainEvent(new UpdatedCommentEvent(UserId, PostId, Content, Status));
        SetModified();
    }
    
    public void Approve()
    {
        Status = CommentStatus.Approved.ToString();
        AddDomainEvent(new UpdatedCommentEvent(UserId, PostId, Content, Status));
        SetModified();
    }
    
    public void MarkAsSpam()
    {
        Status = CommentStatus.Spam.ToString();
        AddDomainEvent(new UpdatedCommentEvent(UserId, PostId, Content, Status));
        SetModified();
    }
    
    public void Restore()
    {
        Status = CommentStatus.Pending.ToString();
        AddDomainEvent(new UpdatedCommentEvent(UserId, PostId, Content, Status));
        SetModified();
    }

    public static CommentStatus MapStatus(string status)
    {
        return status?.Trim().ToLower() switch
        {
            "approved" => CommentStatus.Approved,
            "pending"  => CommentStatus.Pending,
            "spam"     => CommentStatus.Spam,
            _          => CommentStatus.Trash
        };
}

}

public enum CommentStatus
{
    Pending,
    Approved,
    Spam,
    Trash
}

// Map status string to enum in a safe and readable way