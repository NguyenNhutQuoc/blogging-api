using System;
using BloggingSystem.Domain.Commons;

namespace BloggingSystem.Domain.Events
{
    /// <summary>
    /// Event raised when a user is created
    /// </summary>
    public class UserCreatedEvent : DomainEvent
    {
        public long UserId { get; }
        
        public UserCreatedEvent(long userId)
        {
            UserId = userId;
        }
    }
    
    /// <summary>
    /// Event raised when a user is activated
    /// </summary>
    public class UserActivatedEvent : DomainEvent
    {
        public long UserId { get; }
        
        public UserActivatedEvent(long userId)
        {
            UserId = userId;
        }
    }
    
    /// <summary>
    /// Event raised when a user is deactivated
    /// </summary>
    public class UserDeactivatedEvent : DomainEvent
    {
        public long UserId { get; }
        
        public UserDeactivatedEvent(long userId)
        {
            UserId = userId;
        }
    }
    
    /// <summary>
    /// Event raised when a user changes their email
    /// </summary>
    public class UserEmailChangedEvent : DomainEvent
    {
        public long UserId { get; }
        public string NewEmail { get; }
        
        public UserEmailChangedEvent(long userId, string newEmail)
        {
            UserId = userId;
            NewEmail = newEmail;
        }
    }
    
    /// <summary>
    /// Event raised when a user changes their password
    /// </summary>
    public class UserPasswordChangedEvent : DomainEvent
    {
        public long UserId { get; }
        
        public UserPasswordChangedEvent(long userId)
        {
            UserId = userId;
        }
    }
    
    /// <summary>
    /// Event raised when a role is assigned to a user
    /// </summary>
    public class UserRoleAssignedEvent : DomainEvent
    {
        public long UserId { get; }
        public long RoleId { get; }
        
        public UserRoleAssignedEvent(long userId, long roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }
    }
    
    /// <summary>
    /// Event raised when a role is removed from a user
    /// </summary>
    public class UserRoleRemovedEvent : DomainEvent
    {
        public long UserId { get; }
        public long RoleId { get; }
        
        public UserRoleRemovedEvent(long userId, long roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }
    }
    
    /// <summary>
    /// Event raised when a permission is granted to a user
    /// </summary>
    public class UserPermissionGrantedEvent : DomainEvent
    {
        public long UserId { get; }
        public long PermissionId { get; }
        
        public UserPermissionGrantedEvent(long userId, long permissionId)
        {
            UserId = userId;
            PermissionId = permissionId;
        }
    }
    
    /// <summary>
    /// Event raised when a permission is revoked from a user
    /// </summary>
    public class UserPermissionRevokedEvent : DomainEvent
    {
        public long UserId { get; }
        public long PermissionId { get; }
        
        public UserPermissionRevokedEvent(long userId, long permissionId)
        {
            UserId = userId;
            PermissionId = permissionId;
        }
    }
}