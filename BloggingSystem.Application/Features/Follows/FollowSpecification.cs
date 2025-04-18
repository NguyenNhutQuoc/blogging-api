using BloggingSystem.Application.Commons.Specifications;
using BloggingSystem.Domain.Entities;
using MediatR;

namespace BloggingSystem.Application.Features.Follows
{
    public class FollowerSpecification : BaseSpecification<Follower>
    {
        public FollowerSpecification(long followerId, long followingId)
            : base(f => f.FollowerId == followerId && f.FollowingId == followingId)
        {
        }
    }

    public class GetFollowingSpecification : BaseSpecification<Follower>
    {
        public GetFollowingSpecification(long followerId)
            : base(f => f.FollowerId == followerId)
        {
            AddInclude(f => f.Following);
            AddInclude("Following.UserProfile");
            ApplyOrderByDescending(f => f.CreatedAt);
        }

        public GetFollowingSpecification(long followerId, int pageIndex, int pageSize)
            : base(f => f.FollowerId == followerId)
        {
            AddInclude(f => f.Following);
            AddInclude("Following.UserProfile");
            ApplyOrderByDescending(f => f.CreatedAt);
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
        }
    }

    public class GetFollowersSpecification : BaseSpecification<Follower>
    {
        public GetFollowersSpecification(long followingId)
            : base(f => f.FollowingId == followingId)
        {
            AddInclude(f => f.FollowerNavigation);
            AddInclude("FollowerNavigation.UserProfile");
            ApplyOrderByDescending(f => f.CreatedAt);
        }

        public GetFollowersSpecification(long followingId, int pageIndex, int pageSize)
            : base(f => f.FollowingId == followingId)
        {
            AddInclude(f => f.FollowerNavigation);
            AddInclude("FollowerNavigation.UserProfile");
            ApplyOrderByDescending(f => f.CreatedAt);
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
        }
    }
}