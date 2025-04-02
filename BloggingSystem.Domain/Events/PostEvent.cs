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

        public PostUpdatedEvent(long postId, long authorId, string title, string slug)
        {
            PostId = postId;
            AuthorId = authorId;
            Title = title;
            Slug = slug;
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
    /// Event raised when a comment is added to a post
    /// </summary>
    public class CommentAddedEvent : DomainEvent
    {
        public long CommentId { get; }
        public long PostId { get; }
        public long UserId { get; }

        public CommentAddedEvent(long commentId, long postId, long userId)
        {
            CommentId = commentId;
            PostId = postId;
            UserId = userId;
        }
    }
    
    /// <summary>
    /// Event raised when a comment is updated
    /// </summary>
    public class CommentUpdatedEvent : DomainEvent
    {
        public long CommentId { get; }
        public long PostId { get; }
        public long UserId { get; }
        public CommentStatus Status { get; }

        public CommentUpdatedEvent(long commentId, long postId, long userId, CommentStatus status)
        {
            CommentId = commentId;
            PostId = postId;
            UserId = userId;
            Status = status;
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
}