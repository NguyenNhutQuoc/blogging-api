using BloggingSystem.Application.Commons.Specifications;
using BloggingSystem.Domain.Entities;

namespace BloggingSystem.Application.Features.User;

 /// <summary>
/// Specification to get user by ID
/// </summary>
public class UserByIdSpecification : BaseSpecification<Domain.Entities.User>
{
    public UserByIdSpecification(long id) 
        : base(u => u.Id == id)
    {
        // Include user profile
        AddInclude(u => u.UserProfile);
    }
}

/// <summary>
/// Specification to get user by username
/// </summary>
public class UserByUsernameSpecification : BaseSpecification<Domain.Entities.User>
{
    public UserByUsernameSpecification(string username) 
        : base(u => u.Username == username)
    {
        DisableTracking();
    }
}

/// <summary>
/// Specification to get user by email
/// </summary>
public class UserByEmailSpecification : BaseSpecification<Domain.Entities.User>
{
    public UserByEmailSpecification(string email) 
        : base(u => u.Email == email)
    {
        DisableTracking();
    }
}

/// <summary>
/// Specification to get users with a specific role
/// </summary>
public class UsersWithRoleSpecification : BaseSpecification<Domain.Entities.User>
{
    public UsersWithRoleSpecification(long userId) 
        : base(u => u.Id == userId)
    {
        // Include user roles and role
        AddInclude(u => u.UserRoles);
        AddInclude("UserRoles.Role");
        
        // Order by username
        ApplyOrderBy(u => u.Username);
        
        DisableTracking();
    }
    
    public UsersWithRoleSpecification(long roleId, int pageIndex, int pageSize) 
        : base(u => u.UserRoles.Any(ur => ur.RoleId == roleId))
    {
        // Include user roles and role
        AddInclude(u => u.UserRoles);
        AddInclude("UserRoles.Role");
        
        // Order by username
        ApplyOrderBy(u => u.Username);
        
        // Apply pagination
        ApplyPaging((pageIndex - 1) * pageSize, pageSize);
        
        DisableTracking();
    }
}

/// <summary>
/// Specification to search users by term
/// </summary>
public class SearchUsersSpecification : BaseSpecification<Domain.Entities.User>
{
    public SearchUsersSpecification(string searchTerm) 
        : base(u => 
            (u.Username != null && u.Username.Contains(searchTerm)) ||
            (u.Email != null && u.Email.Contains(searchTerm)) ||
            (u.UserProfile != null && u.UserProfile.DisplayName != null && u.UserProfile.DisplayName.Contains(searchTerm)))
    {
        AddInclude(u => u.UserProfile);
        ApplyOrderBy(u => u.Username);
        DisableTracking();
    }
    
    public SearchUsersSpecification(string searchTerm, int pageNumber, int pageSize) 
        : base(u => 
            (u.Username != null && u.Username.Contains(searchTerm)) ||
            (u.Email != null && u.Email.Contains(searchTerm)) ||
            (u.UserProfile != null && u.UserProfile.DisplayName != null && u.UserProfile.DisplayName.Contains(searchTerm)))
    {
        AddInclude(u => u.UserProfile);
        ApplyOrderBy(u => u.Username);
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        DisableTracking();
    }
}

/// <summary>
/// Specification to get user roles
/// </summary>
public class UserRolesSpecification : BaseSpecification<UserRole>
{
    public UserRolesSpecification(long userId)
        : base(ur => ur.UserId == userId)
    {
        AddInclude(ur => ur.Role);
        DisableTracking();
    }
}

/// <summary>
/// Specification to get users with a specific permission
/// </summary>
public class UsersWithPermissionSpecification : BaseSpecification<Domain.Entities.User>
{
    public UsersWithPermissionSpecification(long permissionId)
        : base(u => 
            u.UserPermissions.Any(up => up.PermissionId == permissionId && up.IsGranted == true) ||
            u.UserRoles.Any(ur => ur.Role.RolePermissions.Any(rp => rp.PermissionId == permissionId)))
    {
        // Include user permissions and roles
        AddInclude(u => u.UserPermissions);
        AddInclude(u => u.UserRoles);
        AddInclude("UserRoles.Role.RolePermissions");
        
        // Order by username
        ApplyOrderBy(u => u.Username);
        
        DisableTracking();
    }
}

/// <summary>
/// Specification to get active users
/// </summary>
public class ActiveUsersSpecification : BaseSpecification<Domain.Entities.User>
{
    public ActiveUsersSpecification() 
        : base(u => u.IsActive)
    {
        // Order by registration date
        ApplyOrderByDescending(u => u.CreatedAt);
        
        DisableTracking();
    }
    
    public ActiveUsersSpecification(int pageIndex, int pageSize) 
        : base(u => u.IsActive)
    {
        // Order by registration date
        ApplyOrderByDescending(u => u.CreatedAt);
        
        // Apply pagination
        ApplyPaging((pageIndex - 1) * pageSize, pageSize);
        
        DisableTracking();
    }
}

/// <summary>
/// Specification to get profile by user ID
/// </summary>
public class ProfileByUserIdSpecification : BaseSpecification<UserProfile>
{
    public ProfileByUserIdSpecification(long userId) 
        : base(p => p.UserId == userId)
    {
    }
}

/// <summary>
/// Specification to check if a user has a role
/// </summary>
public class UserRoleSpecification : BaseSpecification<UserRole>
{
    public UserRoleSpecification(long userId, long roleId) 
        : base(ur => ur.UserId == userId && ur.RoleId == roleId)
    {
        DisableTracking();
    }
}