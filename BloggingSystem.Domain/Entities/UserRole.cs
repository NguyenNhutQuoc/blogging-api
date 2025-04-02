using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BloggingSystem.Domain.Commons;
using BloggingSystem.Domain.Events;

namespace BloggingSystem.Domain.Entities;

public partial class UserRole: BaseEntity
{
    [NotMapped]
    public DateTime UpdatedAt { get; set; } 
    public long UserId { get; set; }

    public long RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
    
    private UserRole() {}
    
    public UserRole(long userId, long roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
    
    public static UserRole Create(long userId, long roleId)
    {
        var userRole =  new UserRole
        {
            UserId = userId,
            RoleId = roleId
        };
        
        userRole.AddDomainEvent(new UserRoleCreatedEvent(userId, roleId));
        
        return userRole;
        
    }
    
    public void Update(long roleId)
    {
        RoleId = roleId;
        
        SetModified();
        
        AddDomainEvent(new UserRoleUpdatedEvent(roleId, UserId));
    }
    
}
