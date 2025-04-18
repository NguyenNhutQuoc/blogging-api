using System;
using System.Linq;
using BloggingSystem.Application.Commons.Specifications;
using BloggingSystem.Domain.Entities;

namespace BloggingSystem.Application.Features.Post
{
    /// <summary>
    /// Get post by ID with includes
    /// </summary>
    public class PostSpecification : BaseSpecification<Domain.Entities.Post>
    {
        public PostSpecification(long id)
            : base(p => p.Id == id)
        { 
            AddInclude(p => p.Author);
            AddInclude(p => p.PostCategories);
            AddInclude(p => p.PostTags);
            AddInclude("PostCategories.Category");
            AddInclude("PostTags.Tag");
            ApplyOrderByDescending(p => p.CreatedAt);
        }
    }

    /// <summary>
    /// Get post by slug with includes
    /// </summary>
    public class PostBySlugSpecification : BaseSpecification<Domain.Entities.Post>
    {
        public PostBySlugSpecification(string slug)
            : base(p => p.Slug == slug)
        {
            AddInclude(p => p.Author);
            AddInclude(p => p.PostCategories);
            AddInclude(p => p.PostTags);
            AddInclude(p => p.Comments);
            AddInclude("PostCategories.Category");
            AddInclude("PostTags.Tag");
        }
    }

    /// <summary>
    /// Get all posts with optional paging
    /// </summary>
    public class PostsSpecification : BaseSpecification<Domain.Entities.Post>
    {
        public PostsSpecification()
        {
            AddInclude(p => p.Author);
            ApplyOrderByDescending(p => p.CreatedAt);
            DisableTracking();
        }

        public PostsSpecification(int pageNumber, int pageSize)
            : this()
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        }
    }

    /// <summary>
    /// Get published posts with optional paging
    /// </summary>
    public class PublishedPostsSpecification : BaseSpecification<Domain.Entities.Post>
    {
        public PublishedPostsSpecification()
            : base(p => p.Status == "published")
        {
            AddInclude(p => p.Author);
            ApplyOrderByDescending(p => p.PublishedAt);
            DisableTracking();
        }

        public PublishedPostsSpecification(int pageNumber, int pageSize)
            : this()
        {
            AddInclude(p => p.PostCategories);
            AddInclude(p => p.PostTags);
            AddInclude("PostCategories.Category");
            AddInclude("PostTags.Tag");
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        }
    }

    /// <summary>
    /// Get posts by author ID with optional paging
    /// </summary>
    public class PostsByAuthorSpecification : BaseSpecification<Domain.Entities.Post>
    {
        public PostsByAuthorSpecification(long authorId)
            : base(p => p.AuthorId == authorId)
        {
            AddInclude(p => p.Author);
            ApplyOrderByDescending(p => p.CreatedAt);
            DisableTracking();
        }

        public PostsByAuthorSpecification(long authorId, int pageNumber, int pageSize)
            : this(authorId)
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        }
    }
    
    /// <summary>
    /// Search posts by author ID with optional paging
    /// </summary>
    public class PostsByAuthorSearchSpecification : BaseSpecification<Domain.Entities.Post>
    {
        public PostsByAuthorSearchSpecification(long authorId, string searchTerm)
            : base(p => p.AuthorId == authorId && (p.Title.Contains(searchTerm) || p.Content.Contains(searchTerm) || p.Excerpt.Contains(searchTerm)))
        {
            AddInclude(p => p.Author);
            ApplyOrderByDescending(p => p.CreatedAt);
            DisableTracking();
        }

        public PostsByAuthorSearchSpecification(long authorId, string searchTerm, int pageNumber, int pageSize)
            : this(authorId, searchTerm)
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        }
    }

    /// <summary>
    /// Get posts by category ID with optional paging
    /// </summary>
    public class PostsByCategorySpecification : BaseSpecification<Domain.Entities.Post>
    {
        public PostsByCategorySpecification(long categoryId)
            : base(p => p.PostCategories.Any(c => c.CategoryId == categoryId))
        {
            AddInclude(p => p.Author);
            AddInclude(p => p.PostCategories);
            AddInclude(p => p.PostTags);
            AddInclude("PostCategories.Category");
            AddInclude("PostTags.Tag");
            ApplyOrderByDescending(p => p.CreatedAt);
            DisableTracking();
        }

        public PostsByCategorySpecification(long categoryId, int pageNumber, int pageSize)
            : this(categoryId)
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        }
    }

    /// <summary>
    /// Get posts by tag ID with optional paging
    /// </summary>
    public class PostsByTagSpecification : BaseSpecification<Domain.Entities.Post>
    {
        public PostsByTagSpecification(long tagId)
            : base(p => p.PostTags.Any(t => t.TagId == tagId))
        {
            AddInclude(p => p.Author);
            AddInclude("Tags");
            ApplyOrderByDescending(p => p.CreatedAt);
            DisableTracking();
        }

        public PostsByTagSpecification(long tagId, int pageNumber, int pageSize)
            : this(tagId)
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        }
    }

    /// <summary>
    /// Search posts with optional paging
    /// </summary>
    public class PostSearchSpecification : BaseSpecification<Domain.Entities.Post>
    {
        public PostSearchSpecification(string searchTerm)
            : base(p => p.Title.Contains(searchTerm) || p.Content.Contains(searchTerm) || p.Excerpt.Contains(searchTerm))
        {
            AddInclude(p => p.Author);
            AddInclude(p => p.PostCategories);
            AddInclude("PostCategories.Category");
            AddInclude(p => p.PostTags);
            AddInclude("PostTags.Tag");
            ApplyOrderByDescending(p => p.CreatedAt);
            DisableTracking();
        }

        public PostSearchSpecification(string searchTerm, int pageNumber, int pageSize)
            : this(searchTerm)
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        }
    }
    
    
    public class PublishedPostsByAuthorSpecification: BaseSpecification<Domain.Entities.Post>
    {
        public PublishedPostsByAuthorSpecification(long authorId)
            : base(p => p.AuthorId == authorId && p.Status == "published" && p.PublishedAt <= DateTime.UtcNow)
        {
            AddInclude(p => p.Author);
            ApplyOrderByDescending(p => p.PublishedAt);
            DisableTracking();
        }

        public PublishedPostsByAuthorSpecification(long authorId, int pageNumber, int pageSize)
            : this(authorId)
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        }
    }
    
    public class PostByIdWithAuthorSpecification : BaseSpecification<Domain.Entities.Post>
    {
        public PostByIdWithAuthorSpecification(long id)
            : base(p => p.Id == id)
        {
            AddInclude(p => p.Author);
            AddInclude("Author.UserProfile");
        }
    }
}