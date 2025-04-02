using BloggingSystem.Domain.Commons;

namespace BloggingSystem.Domain.Events;


public class UserRoleCreatedEvent : DomainEvent
{
    public long RoleId { get; }
    public long UserId { get; }
    
    public UserRoleCreatedEvent(long roleId, long userId)
    {
        RoleId = roleId;
        UserId = userId;
    }
}

public class UserRoleUpdatedEvent : DomainEvent
{
    public long RoleId { get; }
    public long UserId { get; }
        
    public UserRoleUpdatedEvent(long roleId, long userId)
    {
        RoleId = roleId;
        UserId = userId;
    }
}