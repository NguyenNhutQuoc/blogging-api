using System;
using System.Collections.Generic;
using System.Text;
using BloggingSystem.Domain.Commons;
using BloggingSystem.Domain.Events;

namespace BloggingSystem.Domain.Entities;

public partial class Post: BaseEntity
{
    public long AuthorId { get; set; }

    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Excerpt { get; set; }

    public string Content { get; set; } = null!;

    public string? FeaturedImageUrl { get; set; }

    public string? Status { get; set; }

    public string? CommentStatus { get; set; }

    public long? ViewsCount { get; set; }

    public DateTime? PublishedAt { get; set; }

    public virtual ICollection<Analytic> Analytics { get; set; } = new List<Analytic>();

    public virtual User Author { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Poll> Polls { get; set; } = new List<Poll>();

    public virtual ICollection<PostCategory> PostCategories { get; set; } = new List<PostCategory>();

    public virtual ICollection<PostMedium> PostMedia { get; set; } = new List<PostMedium>();

    public virtual ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();

    public virtual ICollection<Revision> Revisions { get; set; } = new List<Revision>();

    public virtual ICollection<SavedPost> SavedPosts { get; set; } = new List<SavedPost>();

    public virtual ICollection<SeriesPost> SeriesPosts { get; set; } = new List<SeriesPost>();
    
    // Add these properties
    public string? ETag { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    
    private void UpdateETag()
    {
        // Generate ETag based on content and last modified
        ETag = Convert.ToBase64String(
            System.Security.Cryptography.MD5.HashData(
                Encoding.UTF8.GetBytes($"{UpdatedAt:O}{Title}{Content}")
            )
        );
    }
    
    private Post() {}
    
    public Post(long authorId, string title, string slug, string content, string excerpt)
    {
        AuthorId = authorId;
        Title = title;
        Slug = slug;
        Content = content;
        Excerpt = excerpt;
        CreatedAt = DateTime.UtcNow;
        Status = PostStatus.Draft.ToString();
        CommentStatus = PostCommentStatus.Open.ToString();
    }
    
    public static Post CreatePost(long authorId, string title, string slug, string content, string excerpt)
    {
        var post = new Post(authorId, title, slug, content, excerpt);
        post.AddDomainEvent(new PostCreatedEvent(post.Id, authorId, title, slug));
        return post;
    }
    
    public void Update(string title, string slug, string content, string excerpt, string featuredImageUrl, string status, string commentStatus)
    {
        Title = title;
        Slug = slug;
        Content = content;
        Excerpt = excerpt;
        FeaturedImageUrl = featuredImageUrl;
        Status = status;
        CommentStatus = commentStatus;
        
        SetModified();
        UpdateETag();
        
        AddDomainEvent(new PostUpdatedEvent(Id, AuthorId, title, slug, excerpt, featuredImageUrl, status, commentStatus ));
    }
    
    public void Publish()
    {
        PublishedAt = DateTime.UtcNow;
        Status = PostStatus.Published.ToString();
        
        SetModified();
        UpdateETag();
        
        AddDomainEvent(new PostPublishedEvent(Id, AuthorId, Title, Slug, DateTime.UtcNow));
    }

    public void UnPublish()
    {
        Status = PostStatus.Draft.ToString();
        SetModified();
        AddDomainEvent(new PostUnpublishedEvent(Id, AuthorId, Title, Slug));

    }
    
    public void Schedule(DateTime publishedAt)
    {
        PublishedAt = publishedAt;
        Status = PostStatus.Scheduled.ToString();
        
        SetModified();
        UpdateETag();
        AddDomainEvent(new PostPublishedEvent(Id, AuthorId, Title, Slug, publishedAt));
    }
    
    public void Trash()
    {
        Status = PostStatus.Trash.ToString();
        DeletedAt = DateTime.UtcNow;
        SetModified();
        UpdateETag();

        AddDomainEvent(new PostArchivedEvent(Id, AuthorId, Title, Slug));
    }
    
    public void Restore()
    {
        Status = PostStatus.Draft.ToString();
        DeletedAt = null;
        SetModified();
        UpdateETag();

        AddDomainEvent(new PostUpdatedEvent(Id, AuthorId, Title, Slug, Excerpt, FeaturedImageUrl, Status, CommentStatus));
    }
    
    public void SoftDelete()
    {
    }
    
    public void AddComment(Comment comment)
    {
        Comments.Add(comment);
        SetModified();
        UpdateETag();
    }
    
    public void RemoveComment(Comment comment)
    {
        Comments.Remove(comment);
        SetModified();
            UpdateETag();
    }
    
    public void AddTag(PostTag postTag)
    {
        PostTags.Add(postTag);
        SetModified();
        UpdateETag();
    }
    
    public void RemoveTag(PostTag postTag)
    {
        PostTags.Remove(postTag);
        SetModified();
        UpdateETag();
    }
    
    public void AddCategory(PostCategory postCategory)
    {
        PostCategories.Add(postCategory);
        SetModified();
        UpdateETag();
    }
    
    public void RemoveCategory(PostCategory postCategory)
    {
        PostCategories.Remove(postCategory);
        SetModified();
        UpdateETag();
    }
    
    public void AddMedia(PostMedium postMedium)
    {
        PostMedia.Add(postMedium);
        SetModified();
        UpdateETag();
    }
    
    public void RemoveMedia(PostMedium postMedium)
    {
        PostMedia.Remove(postMedium);
        SetModified();
        UpdateETag();
    }

    public void RestoreFromRevision(long userId, long revisionId, int revisionNumber, string revisionContent, string oldContent) {
        Content = revisionContent;

        AddDomainEvent(new PostRestoredFromRevisionEvent(Id, userId, revisionId, revisionNumber, oldContent, revisionContent));
    }

    public void IncreaseView(string ipAddress, string userAgent) {
        ViewsCount += 1;
        AddDomainEvent(new PostViewedEvent(Id, ipAddress, userAgent));
    }
}

public enum PostStatus
{
    Draft,
    Published,
    Scheduled,
    Private,
    Trash
}

public enum PostCommentStatus
{
    Open,
    Closed
}