using System;
using System.Collections.Generic;

namespace BloggingSystem.Domain.Entities;

public partial class UserPermission
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long PermissionId { get; set; }

    public bool? IsGranted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual User User { get; set; } = null!;
    
    private UserPermission() {}
}
