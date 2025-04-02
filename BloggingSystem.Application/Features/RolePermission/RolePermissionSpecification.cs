using BloggingSystem.Application.Commons.Specifications;

namespace BloggingSystem.Application.Features.RolePermission;

public class RolePermissionByRoleAndPermissionSpecification: BaseSpecification<Domain.Entities.RolePermission>
{
    public RolePermissionByRoleAndPermissionSpecification(long roleId, long permissionId) 
        : base(rp => rp.RoleId == roleId && rp.PermissionId == permissionId)
    {
        // Include role
        AddInclude(rp => rp.Role);
        
        // Include permission
        AddInclude(rp => rp.Permission);
    }
}