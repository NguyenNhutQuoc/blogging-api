using BloggingSystem.Application.Commons.Specifications;
using BloggingSystem.Domain.Entities;

namespace BloggingSystem.Application.Features.SavedPosts
{
    public class SavedPostSpecification : BaseSpecification<SavedPost>
    {
        public SavedPostSpecification(long userId)
            : base(sp => sp.UserId == userId)
        {
        }
        public SavedPostSpecification(long userId, long postId)
            : base(sp => sp.UserId == userId && sp.PostId == postId)
        {
        }
    }
    public class GetSavedPostsSpecification : BaseSpecification<SavedPost>
    {
        public GetSavedPostsSpecification(long userId)
            : base(sp => sp.UserId == userId)
        {
            AddInclude(sp => sp.Post);
            AddInclude(sp => sp.Post.Author);
            ApplyOrderByDescending(sp => sp.CreatedAt);
        }

        public GetSavedPostsSpecification(long userId, int pageIndex, int pageSize)
            : base(sp => sp.UserId == userId)
        {
            AddInclude(sp => sp.Post);
            AddInclude(sp => sp.Post.Author);
            ApplyOrderByDescending(sp => sp.CreatedAt);
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
        }
    }

    public class SearchSavedPostsSpecification : BaseSpecification<SavedPost>
    {
        public SearchSavedPostsSpecification(long userId, string searchTerm)
            : base(sp => 
                sp.UserId == userId && 
                (sp.Post.Title.Contains(searchTerm) || 
                 sp.Post.Content.Contains(searchTerm) || 
                 sp.Post.Excerpt.Contains(searchTerm)))
        {
            AddInclude(sp => sp.Post);
            AddInclude(sp => sp.Post.Author);
            ApplyOrderByDescending(sp => sp.CreatedAt);
        }

        public SearchSavedPostsSpecification(long userId, string searchTerm, int pageIndex, int pageSize)
            : base(sp => 
                sp.UserId == userId && 
                (sp.Post.Title.Contains(searchTerm) || 
                 sp.Post.Content.Contains(searchTerm) || 
                 sp.Post.Excerpt.Contains(searchTerm)))
        {
            AddInclude(sp => sp.Post);
            AddInclude(sp => sp.Post.Author);
            ApplyOrderByDescending(sp => sp.CreatedAt);
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
        }
    }
}