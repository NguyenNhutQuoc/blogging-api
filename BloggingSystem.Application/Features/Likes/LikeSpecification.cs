using BloggingSystem.Application.Commons.Specifications;
using BloggingSystem.Domain.Entities;

namespace BloggingSystem.Application.Features.Likes
{
    public class UserLikedPostsSpecification : BaseSpecification<Like>
    {
        public UserLikedPostsSpecification(long userId)
            : base(l => l.UserId == userId && l.EntityType == "post")
        {
            AddInclude(l => l.User);
            ApplyOrderByDescending(l => l.CreatedAt);
        }

        public UserLikedPostsSpecification(long userId, int pageIndex, int pageSize)
            : base(l => l.UserId == userId && l.EntityType == "post")
        {
            AddInclude(l => l.User);
            ApplyOrderByDescending(l => l.CreatedAt);
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
        }
    }
    public class LikeSpecification : BaseSpecification<Like>
    {
        public LikeSpecification(long userId, long entityId, string entityType)
            : base(l => l.UserId == userId && l.EntityId == entityId && l.EntityType == entityType)
        {
        }
    }
    public class GetEntityLikesSpecification : BaseSpecification<Like>
    {
        public GetEntityLikesSpecification(string entityType, long entityId)
            : base(l => l.EntityType == entityType && l.EntityId == entityId)
        {
            AddInclude(l => l.User);
            AddInclude("User.UserProfile");
            ApplyOrderByDescending(l => l.CreatedAt);
        }

        public GetEntityLikesSpecification(string entityType, long entityId, int pageIndex, int pageSize)
            : base(l => l.EntityType == entityType && l.EntityId == entityId)
        {
            AddInclude(l => l.User);
            AddInclude("User.UserProfile");
            ApplyOrderByDescending(l => l.CreatedAt);
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
        }
    }

    public class GetUserLikedEntitiesSpecification : BaseSpecification<Like>
    {
        public GetUserLikedEntitiesSpecification(long userId, string entityType)
            : base(l => l.UserId == userId && l.EntityType == entityType)
        {
            ApplyOrderByDescending(l => l.CreatedAt);
        }

        public GetUserLikedEntitiesSpecification(long userId, string entityType, int pageIndex, int pageSize)
            : base(l => l.UserId == userId && l.EntityType == entityType)
        {
            if (entityType == "post")
            {
                AddInclude("Post");
                AddInclude("Post.Author");
            }
            else if (entityType == "comment")
            {
                AddInclude("Comment");
                AddInclude("Comment.User");
                AddInclude("Comment.Post");
            }
            
            ApplyOrderByDescending(l => l.CreatedAt);
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
        }
    }
}