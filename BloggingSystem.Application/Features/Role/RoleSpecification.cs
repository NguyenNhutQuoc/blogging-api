using BloggingSystem.Application.Commons.Specifications;
using BloggingSystem.Domain.Entities;

namespace BloggingSystem.Application.Features.Role;

public class RoleByIdSpecification : BaseSpecification<Domain.Entities.Role>
{
    public RoleByIdSpecification(long id) 
        : base(r => r.Id == id)
    {
        // Include role users
        AddInclude(r => r.UserRoles);
        
        // Order by role name
        ApplyOrderBy(r => r.Name);
    }
}

public class RoleBySlugSpecification : BaseSpecification<Domain.Entities.Role>
{
    public RoleBySlugSpecification(string slug) 
        : base(r => r.Slug == slug)
    {
        // Include role users
        AddInclude(r => r.UserRoles);
        
        // Include role permissions
        AddInclude(r => r.RolePermissions);
        
        // Then include permissions for each role permission
        AddInclude($"{nameof(Domain.Entities.Role.RolePermissions)}.{nameof(Domain.Entities.RolePermission.Permission)}");
        
        // Order by role name
        ApplyOrderBy(r => r.Name);
    }
}
public class RoleByNameSpecification : BaseSpecification<Domain.Entities.Role>
{
    public RoleByNameSpecification(string name) 
        : base(r => r.Name == name)
    {
        // Include role users
        AddInclude(r => r.UserRoles);
        
        // Order by role name
        ApplyOrderBy(r => r.Name);
    }
}

public class RolesWithUserSpecification : BaseSpecification<Domain.Entities.Role>
{
    public RolesWithUserSpecification(long userId) 
        : base(r => r.UserRoles.Any(ur => ur.UserId == userId))
    {
        // Include role users
        AddInclude(r => r.UserRoles);
        AddInclude(r => r.RolePermissions);
        // Inclue Permissions
        AddInclude($"{nameof(Domain.Entities.Role.RolePermissions)}.{nameof(Domain.Entities.RolePermission.Permission)}");
        // Order by role name
        ApplyOrderBy(r => r.Name);
    }
    public RolesWithUserSpecification(long userId, int pageIndex, int pageSize) 
        : base(r => r.UserRoles.Any(ur => ur.UserId == userId))
    {
        // Include role users
        AddInclude(r => r.UserRoles);
        
        // Order by role name
        ApplyOrderBy(r => r.Name);
    }
}

public class RoleSearchSpecification : BaseSpecification<Domain.Entities.Role>
{
    public RoleSearchSpecification(string search, int pageIndex, int pageSize) 
        : base(r => r.Name.Contains(search) || r.Slug.Contains(search) || r.Description.Contains(search))
    {
        // Order by role name
        ApplyOrderBy(r => r.Name);
        
        // Apply pagination
        ApplyPaging((pageIndex - 1) * pageSize, pageSize);
    }
    public RoleSearchSpecification(string search) 
        : base(r => r.Name.Contains(search) || r.Slug.Contains(search) || r.Description.Contains(search))
    {
        // Order by role name
        ApplyOrderBy(r => r.Name);
    }
}

public class RoleSpecification : BaseSpecification<Domain.Entities.Role>
{
    public RoleSpecification()
    {
        // Include role users
        AddInclude(r => r.UserRoles);
        
        // Order by role name
        ApplyOrderBy(r => r.Name);
    }
    
    public RoleSpecification(int pageIndex, int pageSize) 
        : base()
    {
        // Include role users
        AddInclude(r => r.UserRoles);
        
        // Order by role name
        ApplyOrderBy(r => r.Name);
        
        // Apply pagination
        ApplyPaging((pageIndex - 1) * pageSize, pageSize);
    }
}