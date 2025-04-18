using System;
using System.Collections.Generic;
using BloggingSystem.Domain.Commons;
using BloggingSystem.Domain.Events;
using BloggingSystem.Shared.Utils;

namespace BloggingSystem.Domain.Entities;

public partial class Permission: BaseEntity
{
    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string Module { get; set; } = null!;

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    
    private Permission() {}
    
    public Permission(string name, string module, string? description = null)
    {
        Name = name;
        Slug = StringUtils.ToSlug(name);
        Module = module;
        Description = description;
    }
    
    public static Permission Create(string name, string module, string? description = null)
    {
        var permission = new Permission
        {
            Name = name,
            Slug = StringUtils.ToSlug(name),
            Module = module,
            Description = description
        };
        
        permission.AddDomainEvent(new CreatedPermissionEvent(permission.Id, permission.Name));
        
        
        return permission;
    }
    
    public void Update(string name, string module, string? description = null)
    {
        Name = name;
        Slug = StringUtils.ToSlug(name);
        Module = module;
        Description = description;
        
        SetModified();
        AddDomainEvent(new UpdatedPermissionEvent(Id, Name));
    }
    
    public void Delete()
    {
        AddDomainEvent(new DeletedPermissionEvent(Id));
    }
}
