using System;
using System.Collections.Generic;

namespace BloggingSystem.Domain.Entities;

public partial class ApiToken
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string Token { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Permissions { get; set; }

    public DateTime? LastUsedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
    
    private ApiToken() {}
}
