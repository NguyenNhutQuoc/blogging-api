using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BloggingSystem.Domain.Commons;

namespace BloggingSystem.Domain.Entities;

public partial class AuditLog: BaseEntity
{
    public long? UserId { get; set; }

    public string Action { get; set; } = null!;

    public string EntityType { get; set; } = null!;

    public long EntityId { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }
    
    [NotMapped]
    public DateTime? UpdatedAt { get; set; }
    
    private AuditLog() {}
    
    public AuditLog(long? userId, string action, string entityType, long entityId, string? oldValues, string? newValues, string? ipAddress, string? userAgent)
    {
        UserId = userId;
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
        OldValues = oldValues;
        NewValues = newValues;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }
    
    public static AuditLog Create(long? userId, string action, string entityType, long entityId, string? oldValues, string? newValues, string? ipAddress, string? userAgent)
    {
        return new AuditLog(userId, action, entityType, entityId, oldValues, newValues, ipAddress, userAgent);
    }
}
