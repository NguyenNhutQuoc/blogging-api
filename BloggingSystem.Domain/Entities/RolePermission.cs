using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BloggingSystem.Domain.Commons;
using BloggingSystem.Domain.Events;

namespace BloggingSystem.Domain.Entities;

public partial class RolePermission: BaseEntity
{
    [NotMapped]
    public DateTime UpdatedAt { get; set; }
    public long Id { get; set; }

    public long RoleId { get; set; }

    public long PermissionId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
    
    private RolePermission() {}
    
    public RolePermission(long roleId, long permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }

    public static RolePermission Create(long roleId, long permissionId) {
        var rolePermission = new RolePermission(roleId, permissionId);

        rolePermission.AddDomainEvent(new GrantedPermissionToRoleEvent(roleId, permissionId));

        return rolePermission;
    }

    public void Delete(long roleId, long permissionId) {
        AddDomainEvent(new RevokedPermissionFromRoleEvent(roleId, permissionId));
    }
}
