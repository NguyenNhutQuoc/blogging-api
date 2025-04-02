using System;
using System.Collections.Generic;
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
    
    public void Update(string title, string slug, string content)
    {
        Title = title;
        Slug = slug;
        Content = content;
        
        SetModified();
        
        AddDomainEvent(new PostUpdatedEvent(Id, AuthorId, title, slug));
    }
    
    public void Publish(DateTime publishedAt)
    {
        PublishedAt = publishedAt;
        Status = PostStatus.Published.ToString();
        
        SetModified();
        
        AddDomainEvent(new PostPublishedEvent(Id, AuthorId, Title, Slug, publishedAt));
    }
    
    public void Schedule(DateTime publishedAt)
    {
        PublishedAt = publishedAt;
        Status = PostStatus.Scheduled.ToString();
        
        SetModified();
        AddDomainEvent(new PostPublishedEvent(Id, AuthorId, Title, Slug, publishedAt));
    }
    
    public void Trash()
    {
        Status = PostStatus.Trash.ToString();
        SetModified();
    }
    
    public void Restore()
    {
        Status = PostStatus.Draft.ToString();
        SetModified();
    }
    
    public void AddComment(Comment comment)
    {
        Comments.Add(comment);
        SetModified();
    }
    
    public void RemoveComment(Comment comment)
    {
        Comments.Remove(comment);
        SetModified();
    }
    
    public void AddTag(PostTag postTag)
    {
        PostTags.Add(postTag);
        SetModified();
    }
    
    public void RemoveTag(PostTag postTag)
    {
        PostTags.Remove(postTag);
        SetModified();
    }
    
    public void AddCategory(PostCategory postCategory)
    {
        PostCategories.Add(postCategory);
        SetModified();
    }
    
    public void RemoveCategory(PostCategory postCategory)
    {
        PostCategories.Remove(postCategory);
        SetModified();
    }
    
    public void AddMedia(PostMedium postMedium)
    {
        PostMedia.Add(postMedium);
        SetModified();
    }
    
    public void RemoveMedia(PostMedium postMedium)
    {
        PostMedia.Remove(postMedium);
        SetModified();
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