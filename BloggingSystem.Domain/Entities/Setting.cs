using System;
using System.Collections.Generic;

namespace BloggingSystem.Domain.Entities;

public partial class Setting
{
    public long Id { get; set; }

    public string SettingKey { get; set; } = null!;

    public string? SettingValue { get; set; }

    public string? SettingGroup { get; set; }

    public bool? IsPublic { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
    
    private Setting() {}
}
