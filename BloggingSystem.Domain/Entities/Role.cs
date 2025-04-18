using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BloggingSystem.Domain.Commons;
using BloggingSystem.Domain.Events;
using BloggingSystem.Domain.Exceptions;
using BloggingSystem.Shared.Constants;
using BloggingSystem.Shared.Utils;

namespace BloggingSystem.Domain.Entities;

public partial class Role: BaseEntity
{
    private string _name;
    private string _slug;
    private string _description;
    private List<RolePermission> _rolePermissions = new List<RolePermission>();
    private List<UserRole> _userRoles = new List<UserRole>();

    public string Name
    {
        get => _name;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Role name cannot be empty");
            _name = value;
        }
    }

    public string Slug
    {
        get => _slug;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Role slug cannot be empty");
            _slug = value;
        }
    }

    public string Description
    {
        get => _description;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Role description cannot be empty");
            _description = value;
        }
    }

    public virtual ICollection<RolePermission>? RolePermissions { get; set; } = new List<RolePermission>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    
    private Role() {}
    
    private Role(string name, string description)
    {
        Name = name;
        Slug = StringUtils.ToSlug(Name);
        Description = description;
    }
    
    public static Role Create(string name, string description)
    {
        var role =  new Role(name, description);
        role.ValidateState();

        role.AddDomainEvent(new CreatedRoleEvent(role.Id, role.Name));
        return role;
    }
    
    public void Update(string name, string description)
    {
        Name = name;
        Slug = StringUtils.ToSlug(Name);
        Description = description;
        ValidateState();
        SetModified();
        
        AddDomainEvent(new UpdatedRoleEvent(Id, Name));
    }

    private void ValidateState()
    {
        var isRightSlug = Regex.IsMatch(Slug, AppConstants.RegexPatterns.Slug);
        if (!isRightSlug)
            throw new DomainException("Invalid slug format");
    }
}
