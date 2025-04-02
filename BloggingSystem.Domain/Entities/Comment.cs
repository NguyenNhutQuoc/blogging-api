using System;
using System.Collections.Generic;
using BloggingSystem.Domain.Commons;

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
        return new Comment(postId, userId, content);
    }
    
    public void Update(string content)
    {
        Content = content;
        
        SetModified();
    }
    
    public void UpdateStatus(CommentStatus status)
    {
        Status = status.ToString();
        
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
        
        SetModified();
    }
    
    public void Approve()
    {
        Status = CommentStatus.Approved.ToString();
        
        SetModified();
    }
    
    public void MarkAsSpam()
    {
        Status = CommentStatus.Spam.ToString();
        
        SetModified();
    }
    
    public void Restore()
    {
        Status = CommentStatus.Pending.ToString();
        
        SetModified();
    }
}

public enum CommentStatus
{
    Pending,
    Approved,
    Spam,
    Trash
}
