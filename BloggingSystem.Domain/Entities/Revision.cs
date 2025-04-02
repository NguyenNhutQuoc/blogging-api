using System;
using System.Collections.Generic;

namespace BloggingSystem.Domain.Entities;

public partial class Revision
{
    public long Id { get; set; }

    public long PostId { get; set; }

    public long UserId { get; set; }

    public string Content { get; set; } = null!;

    public int RevisionNumber { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
    
    private Revision() {}
}
