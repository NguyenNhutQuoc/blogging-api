using BloggingSystem.Domain.Commons;

namespace BloggingSystem.Domain.Events
{
    #region Follow Events
    
    public class UserFollowedEvent : DomainEvent
    {
        public long FollowerId { get; }
        public long FollowingId { get; }

        public UserFollowedEvent(long followerId, long followingId)
        {
            FollowerId = followerId;
            FollowingId = followingId;
        }
    }

    public class UserUnfollowedEvent : DomainEvent
    {
        public long FollowerId { get; }
        public long FollowingId { get; }

        public UserUnfollowedEvent(long followerId, long followingId)
        {
            FollowerId = followerId;
            FollowingId = followingId;
        }
    }

    #endregion

    #region Like Events

    public class EntityLikedEvent : DomainEvent
    {
        public long UserId { get; }
        public string EntityType { get; }
        public long EntityId { get; }

        public EntityLikedEvent(long userId, string entityType, long entityId)
        {
            UserId = userId;
            EntityType = entityType;
            EntityId = entityId;
        }
    }

    public class EntityUnlikedEvent : DomainEvent
    {
        public long UserId { get; }
        public string EntityType { get; }
        public long EntityId { get; }

        public EntityUnlikedEvent(long userId, string entityType, long entityId)
        {
            UserId = userId;
            EntityType = entityType;
            EntityId = entityId;
        }
    }

    #endregion

    #region SavedPost Events

    public class PostSavedEvent : DomainEvent
    {
        public long UserId { get; }
        public long PostId { get; }

        public PostSavedEvent(long userId, long postId)
        {
            UserId = userId;
            PostId = postId;
        }
    }

    public class PostUnsavedEvent : DomainEvent
    {
        public long UserId { get; }
        public long PostId { get; }

        public PostUnsavedEvent(long userId, long postId)
        {
            UserId = userId;
            PostId = postId;
        }
    }

    #endregion
}