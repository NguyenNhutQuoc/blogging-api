using BloggingSystem.Application.Commons.Specifications;

namespace BloggingSystem.Application.Features.Permission;

public class PermissionsSpecification : BaseSpecification<Domain.Entities.Permission>
{
    public PermissionsSpecification() 
        : base()
    {
        // Include permission roles`
        AddInclude(p => p.RolePermissions);
        
        // Order by permission name
        ApplyOrderBy(p => p.Name);
    }
    
    public PermissionsSpecification(int pageIndex, int pageSize) 
        : base()
    {
        // Include permission roles
        AddInclude(p => p.RolePermissions);
        
        // Order by permission name
        ApplyOrderBy(p => p.Name);
        
        ApplyPaging((pageIndex - 1) * pageSize, pageSize);
    }
}

public class CreatedPermissionSpecification : BaseSpecification<Domain.Entities.Permission>
{
    public CreatedPermissionSpecification(long permissionId) 
        : base(p => p.Id == permissionId)
    {
        // Include permission roles
        AddInclude(p => p.RolePermissions);
        
        // Order by permission name
        ApplyOrderBy(p => p.Name);
    }
}

public class PermissionByIdSpecification : BaseSpecification<Domain.Entities.Permission>
{
    public PermissionByIdSpecification(long id) 
        : base(p => p.Id == id)
    {
        // Include permission roles
        AddInclude(p => p.RolePermissions);
        
        // Order by permission name
        ApplyOrderBy(p => p.Name);
    }
}

public class PermissionBySlugSpecification : BaseSpecification<Domain.Entities.Permission>
{
    public PermissionBySlugSpecification(string slug) 
        : base(p => p.Slug == slug)
    {
        // Include permission roles
        AddInclude(p => p.RolePermissions);
        
        // Order by permission name
        ApplyOrderBy(p => p.Name);
    }
}

public class PermissionByNameSpecification : BaseSpecification<Domain.Entities.Permission>
{
    public PermissionByNameSpecification(string name) 
        : base(p => p.Name == name)
    {
        // Include permission roles
        AddInclude(p => p.RolePermissions);
        
        // Order by permission name
        ApplyOrderBy(p => p.Name);
    }
}

public class PermissionByModuleSpecification : BaseSpecification<Domain.Entities.Permission>
{
    public PermissionByModuleSpecification(string module) 
        : base(p => p.Module == module)
    {
        // Include permission roles
        AddInclude(p => p.RolePermissions);
        
        // Order by permission name
        ApplyOrderBy(p => p.Name);
    }
    
    public PermissionByModuleSpecification(string module, int pageIndex, int pageSize) 
        : base(p => p.Module == module)
    {
        // Include permission roles
        AddInclude(p => p.RolePermissions);
        
        // Order by permission name
        ApplyOrderBy(p => p.Name);
        
        ApplyPaging((pageIndex - 1) * pageSize, pageSize);
    }
}

public class PermissionsWithRoleSpecification : BaseSpecification<Domain.Entities.Permission>
{
    public PermissionsWithRoleSpecification(long roleId) 
        : base(p => p.RolePermissions.Any(rp => rp.RoleId == roleId))
    {
        // Include permission roles and role
        AddInclude(p => p.RolePermissions);
        AddInclude("RolePermissions.Role");
        
        // Order by permission name
        ApplyOrderBy(p => p.Name);
    }
    
    public PermissionsWithRoleSpecification(long roleId, int pageIndex, int pageSize) 
        : base(p => p.RolePermissions.Any(rp => rp.RoleId == roleId))
    {
        // Include permission roles and role
        AddInclude(p => p.RolePermissions);
        AddInclude("RolePermissions.Role");
        
        // Order by permission name
        ApplyOrderBy(p => p.Name);
        
        ApplyPaging((pageIndex - 1) * pageSize, pageSize);
    }
}

public class UpdatedPermissionSpecification : BaseSpecification<Domain.Entities.Permission>
{
    public UpdatedPermissionSpecification(long permissionId) 
        : base(p => p.Id == permissionId)
    {
        // Include permission roles
        AddInclude(p => p.RolePermissions);
        
        // Order by permission name
        ApplyOrderBy(p => p.Name);
    }
}

public class UserPermissionSpecification : BaseSpecification<Domain.Entities.Permission>
{
    public UserPermissionSpecification(long userId) 
        : base(p => p.UserPermissions.Any(up => up.UserId == userId))
    {
        // Include permission users
        AddInclude(p => p.UserPermissions);
        
        // Order by permission name
        ApplyOrderBy(p => p.Name);
    }
    
    public UserPermissionSpecification(long userId, int pageIndex, int pageSize) 
        : base(p => p.UserPermissions.Any(up => up.UserId == userId))
    {
        // Include permission users
        AddInclude(p => p.UserPermissions);
        
        // Order by permission name
        ApplyOrderBy(p => p.Name);
        
        ApplyPaging((pageIndex - 1) * pageSize, pageSize);
    }
}

public class PermissionSearchSpecification : BaseSpecification<Domain.Entities.Permission>
{
    public PermissionSearchSpecification(string searchTerm) 
        : base(p => p.Name.Contains(searchTerm) || p.Slug.Contains(searchTerm))
    {
        // Include permission roles
        AddInclude(p => p.RolePermissions);
        
        // Order by permission name
        ApplyOrderBy(p => p.Name);
    }
    
    public PermissionSearchSpecification(string searchTerm, int pageIndex, int pageSize) 
        : base(p => p.Name.Contains(searchTerm) || p.Slug.Contains(searchTerm) || p.Description.Contains(searchTerm) || p.Module.Contains(searchTerm))
    {
        // Include permission roles
        AddInclude(p => p.RolePermissions);
        
        // Order by permission name
        ApplyOrderBy(p => p.Name);
        
        ApplyPaging((pageIndex - 1) * pageSize, pageSize);
    }
}