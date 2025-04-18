using System;
using System.Linq;
using BloggingSystem.Application.Commons.Specifications;
using BloggingSystem.Domain.Entities;

namespace BloggingSystem.Application.Features.Comment
{
    /// <summary>
    /// Get comment by ID with includes
    /// </summary>
    public class CommentByIdSpecification : BaseSpecification<Domain.Entities.Comment>
    {
        public CommentByIdSpecification(long id)
            : base(c => c.Id == id)
        {
            AddInclude(c => c.User);
            AddInclude(c => c.Post);
        }
    }

    /// <summary>
    /// Get comments by post ID with optional paging
    /// </summary>
    public class CommentsByPostSpecification : BaseSpecification<Domain.Entities.Comment>
    {
        public CommentsByPostSpecification(long postId)
            : base(c => c.PostId == postId)
        {
            AddInclude(c => c.User);
            ApplyOrderBy(c => c.CreatedAt);
            DisableTracking();
        }

        public CommentsByPostSpecification(long postId, int pageNumber, int pageSize)
            : this(postId)
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        }
    }

    /// <summary>
    /// Get comments by status with optional paging
    /// </summary>
    public class CommentsByStatusSpecification : BaseSpecification<Domain.Entities.Comment>
    {
        public CommentsByStatusSpecification(string status)
            : base(c => c.Status == status)
        {
            AddInclude(c => c.User);
            AddInclude(c => c.Post);
            ApplyOrderByDescending(c => c.CreatedAt);
            DisableTracking();
        }

        public CommentsByStatusSpecification(string status, int pageNumber, int pageSize)
            : this(status)
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        }
    }

    /// <summary>
    /// Get comments by user ID with optional paging
    /// </summary>
    public class CommentsByUserSpecification : BaseSpecification<Domain.Entities.Comment>
    {
        public CommentsByUserSpecification(long userId)
            : base(c => c.UserId == userId)
        {
            AddInclude(c => c.Post);
            ApplyOrderByDescending(c => c.CreatedAt);
            DisableTracking();
        }

        public CommentsByUserSpecification(long userId, int pageNumber, int pageSize)
            : this(userId)
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        }
    }

    /// <summary>
    /// Get top-level comments (no parent) by post ID with optional paging
    /// </summary>
    public class TopLevelCommentsByPostSpecification : BaseSpecification<Domain.Entities.Comment>
    {
        public TopLevelCommentsByPostSpecification(long postId)
            : base(c => c.PostId == postId && c.ParentId == null)
        {
            AddInclude(c => c.User);
            ApplyOrderBy(c => c.CreatedAt);
            DisableTracking();
        }

        public TopLevelCommentsByPostSpecification(long postId, int pageNumber, int pageSize)
            : this(postId)
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        }
    }

    /// <summary>
    /// Get replies to a comment
    /// </summary>
    public class CommentRepliesSpecification : BaseSpecification<Domain.Entities.Comment>
    {
        public CommentRepliesSpecification(long parentId)
            : base(c => c.ParentId == parentId)
        {
            AddInclude(c => c.User);
            ApplyOrderBy(c => c.CreatedAt);
            DisableTracking();
        }

        public CommentRepliesSpecification(long parentId, int pageNumber, int pageSize)
            : this(parentId)
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        }
    }

    /// <summary>
    /// Get all comments with optional paging
    /// </summary>
    public class CommentsSpecification : BaseSpecification<Domain.Entities.Comment>
    {
        public CommentsSpecification()
        {
            AddInclude(c => c.User);
            AddInclude(c => c.Post);
            ApplyOrderByDescending(c => c.CreatedAt);
            DisableTracking();
        }

        public CommentsSpecification(int pageNumber, int pageSize)
            : this()
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        }
    }

    /// <summary>
    /// Search comments with optional paging
    /// </summary>
    public class CommentSearchSpecification : BaseSpecification<Domain.Entities.Comment>
    {
        public CommentSearchSpecification(string searchTerm)
            : base(c => c.Content.Contains(searchTerm))
        {
            AddInclude(c => c.User);
            AddInclude(c => c.Post);
            ApplyOrderByDescending(c => c.CreatedAt);
            DisableTracking();
        }

        public CommentSearchSpecification(string searchTerm, int pageNumber, int pageSize)
            : this(searchTerm)
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        }
    }

    public class CommentCountByStatusSpecification : BaseSpecification<Domain.Entities.Comment>
    {
        public CommentCountByStatusSpecification(string status)
            : base(c => c.Status == status)
        {
            ApplyOrderByDescending(c => c.CreatedAt);
            DisableTracking();
        }
    }
    
    public class TopLevelCommentsSpecification : BaseSpecification<Domain.Entities.Comment>
    {
        public TopLevelCommentsSpecification()
            : base(c => c.ParentId == null)
        {
        }

        public TopLevelCommentsSpecification(int pageIndex, int pageSize)
            : base(c => c.ParentId == null)
        {
            ApplyOrderBy(c => c.CreatedAt);
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
        }
    }

    public class CommentRepliesForParentsSpecification : BaseSpecification<Domain.Entities.Comment>
    {
        public CommentRepliesForParentsSpecification(List<long> parentIds)
            : base(c => c.ParentId.HasValue && parentIds.Contains(c.ParentId.Value))
        {
            ApplyOrderBy(c => c.CreatedAt);
        }
    }
}