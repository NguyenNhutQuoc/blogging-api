using System;
using BloggingSystem.Domain.Commons;
using BloggingSystem.Domain.Entities;

namespace BloggingSystem.Domain.Events
{
    /// <summary>
    /// Event raised when a post is created
    /// </summary>
    public class PostCreatedEvent : DomainEvent
    {
        public long PostId { get; }
        public long AuthorId { get; }
        public string Title { get; }
        public string Slug { get; }

        public PostCreatedEvent(long postId, long authorId, string title, string slug)
        {
            PostId = postId;
            AuthorId = authorId;
            Title = title;
            Slug = slug;
        }
    }

    /// <summary>
    /// Event raised when a post is published
    /// </summary>
    public class PostPublishedEvent : DomainEvent
    {
        public long PostId { get; }
        public long AuthorId { get; }
        public string Title { get; }
        public string Slug { get; }
        public DateTime PublishedAt { get; }

        public PostPublishedEvent(long postId, long authorId, string title, string slug, DateTime publishedAt)
        {
            PostId = postId;
            AuthorId = authorId;
            Title = title;
            Slug = slug;
            PublishedAt = publishedAt;
        }
    }

    /// <summary>
    /// Event raised when a post is updated
    /// </summary>
    public class PostUpdatedEvent : DomainEvent
    {
        public long PostId { get; }
        public long AuthorId { get; }
        public string Title { get; }
        public string Slug { get; }
        public string Excerpt {get; }
        public string FeaturedImageUrl {get;}
        public string Status {get;}
        public string CommentStatus {get;}

        public PostUpdatedEvent(long postId, long authorId, string title, string slug, string excerpt, string featuredImageUrl, string status, string commentStatus)
        {
            PostId = postId;
            AuthorId = authorId;
            Title = title;
            Slug = slug;
            Excerpt = excerpt;
            FeaturedImageUrl = featuredImageUrl;
            Status = status;
            CommentStatus = commentStatus;
        }
    }

    /// <summary>
    /// Event raised when a post is archived/deleted
    /// </summary>
    public class PostArchivedEvent : DomainEvent
    {
        public long PostId { get; }
        public long AuthorId { get; }
        public string Title { get; }
        public string Slug { get; }

        public PostArchivedEvent(long postId, long authorId, string title, string slug)
        {
            PostId = postId;
            AuthorId = authorId;
            Title = title;
            Slug = slug;
        }
    }

    /// <summary>
    /// Event raised when a post view is recorded
    /// </summary>
    public class PostViewedEvent : DomainEvent
    {
        public long PostId { get; }
        public string? IPAddress { get; }
        public string? UserAgent { get; }

        public PostViewedEvent(long postId, string? ipAddress, string? userAgent)
        {
            PostId = postId;
            IPAddress = ipAddress;
            UserAgent = userAgent;
        }
    }
    
    public class PostUnpublishedEvent : DomainEvent
    {
        public long PostId { get; }
        public long AuthorId { get; }
        public string Title { get; }
        public string Slug { get; }

        public PostUnpublishedEvent(long postId, long authorId, string title, string slug)
        {
            PostId = postId;
            AuthorId = authorId;
            Title = title;
            Slug = slug;
        }
    }
    
    public class PostRestoredFromRevisionEvent : DomainEvent
    {
        public long PostId { get; }
        public long UserId { get; }
        public long RevisionId { get; }
        public int RevisionNumber { get; }
        public string OldContent { get; }
        public string RestoredContent { get; }

        public PostRestoredFromRevisionEvent(
            long postId,
            long userId,
            long revisionId,
            int revisionNumber,
            string oldContent,
            string restoredContent)
        {
            PostId = postId;
            UserId = userId;
            RevisionId = revisionId;
            RevisionNumber = revisionNumber;
            OldContent = oldContent;
            RestoredContent = restoredContent;
        }
    }
}
