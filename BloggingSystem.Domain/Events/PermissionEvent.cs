using BloggingSystem.Domain.Commons;

namespace BloggingSystem.Domain.Events;

public class CreatedPermissionEvent : DomainEvent
{
    public long PermissionId { get; }
    public string PermissionName { get; }
    
    public CreatedPermissionEvent(long permissionId, string permissionName)
    {
        PermissionId = permissionId;
        PermissionName = permissionName;
    }
}

public class UpdatedPermissionEvent : DomainEvent
{
    public long PermissionId { get; }
    public string PermissionName { get; }
        
    public UpdatedPermissionEvent(long permissionId, string permissionName)
    {
        PermissionId = permissionId;
        PermissionName = permissionName;
    }
}

public class DeletedPermissionEvent : DomainEvent
{
    public long PermissionId { get; }
        
    public DeletedPermissionEvent(long permissionId)
    {
        PermissionId = permissionId;
    }
}