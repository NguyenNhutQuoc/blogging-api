using System;
using System.Collections.Generic;

namespace BloggingSystem.Domain.Entities;

public partial class Medium
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public string MimeType { get; set; } = null!;

    public long FileSize { get; set; }

    public string? AltText { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<PostMedium> PostMedia { get; set; } = new List<PostMedium>();

    public virtual User User { get; set; } = null!;
    
    private Medium() {}
}
