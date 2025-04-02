using BloggingSystem.Domain.Commons;

namespace BloggingSystem.Domain.Events;

public class CreatedRoleEvent : DomainEvent
{
    public long RoleId { get; }
    public string RoleName { get; }
    
    public CreatedRoleEvent(long roleId, string roleName)
    {
        RoleId = roleId;
        RoleName = roleName;
    }
}

public class UpdatedRoleEvent : DomainEvent
{
    public long RoleId { get; }
    public string RoleName { get; }
        
    public UpdatedRoleEvent(long roleId, string roleName)
    {
        RoleId = roleId;
        RoleName = roleName;
    }
}

public class GrantedPermissionToRoleEvent : DomainEvent
{
    public long RoleId { get; }
    public long PermissionId { get; }
    
    public GrantedPermissionToRoleEvent(long roleId, long permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }
}

public class RevokedPermissionFromRoleEvent : DomainEvent
{
    public long RoleId { get; }
    public long PermissionId { get; }
    
    public RevokedPermissionFromRoleEvent(long roleId, long permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }
}

public class GrantedBulkPermissionsToRoleEvent : DomainEvent
{
    public long RoleId { get; }
    public List<long> PermissionIds { get; }
    
    public GrantedBulkPermissionsToRoleEvent(long roleId, List<long> permissionIds)
    {
        RoleId = roleId;
        PermissionIds = permissionIds;
    }
}

public class RevokedBulkPermissionsFromRoleEvent : DomainEvent
{
    public long RoleId { get; }
    public List<long> PermissionIds { get; }
    
    public RevokedBulkPermissionsFromRoleEvent(long roleId, List<long> permissionIds)
    {
        RoleId = roleId;
        PermissionIds = permissionIds;
    }
}
    
public class DeletedRoleEvent : DomainEvent
{
    public long RoleId { get; }
        
    public DeletedRoleEvent(long roleId)
    {
        RoleId = roleId;
    }
}