using System;
using BloggingSystem.Domain.Commons;

namespace BloggingSystem.Domain.Events
{
    /// <summary>
    /// Event raised when a user profile is created
    /// </summary>
    public class UserProfileCreatedEvent : DomainEvent
    {
        public long ProfileId { get; }
        public long UserId { get; }
        
        public UserProfileCreatedEvent(long profileId, long userId)
        {
            ProfileId = profileId;
            UserId = userId;
        }
    }
    
    /// <summary>
    /// Event raised when a user profile is updated
    /// </summary>
    public class UserProfileUpdatedEvent : DomainEvent
    {
        public long ProfileId { get; }
        public long UserId { get; }
        
        public UserProfileUpdatedEvent(long profileId, long userId)
        {
            ProfileId = profileId;
            UserId = userId;
        }
    }
    
    /// <summary>
    /// Event raised when a user avatar is updated
    /// </summary>
    public class UserAvatarUpdatedEvent : DomainEvent
    {
        public long ProfileId { get; }
        public long UserId { get; }
        
        public UserAvatarUpdatedEvent(long profileId, long userId)
        {
            ProfileId = profileId;
            UserId = userId;
        }
    }
    
    /// <summary>
    /// Event raised when user social links are updated
    /// </summary>
    public class UserSocialLinksUpdatedEvent : DomainEvent
    {
        public long ProfileId { get; }
        public long UserId { get; }
        
        public UserSocialLinksUpdatedEvent(long profileId, long userId)
        {
            ProfileId = profileId;
            UserId = userId;
        }
    }
}