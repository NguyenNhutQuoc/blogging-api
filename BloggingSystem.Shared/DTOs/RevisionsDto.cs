using System;

namespace BloggingSystem.Shared.DTOs
{
    public class RevisionDto
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public string PostTitle { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public int RevisionNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RevisionComparisonDto
    {
        public long SourceRevisionId { get; set; }
        public long TargetRevisionId { get; set; }
        public int SourceRevisionNumber { get; set; }
        public int TargetRevisionNumber { get; set; }
        public string SourceUser { get; set; }
        public string TargetUser { get; set; }
        public DateTime SourceCreatedAt { get; set; }
        public DateTime TargetCreatedAt { get; set; }
        public long PostId { get; set; }
        public List<RevisionDiffLineDto> Changes { get; set; }
        public int LinesAdded { get; set; }
        public int LinesRemoved { get; set; }
        public int LinesChanged { get; set; }
    }

    public class RevisionDiffLineDto
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public int Position { get; set; }
        public List<RevisionDiffSubPieceDto> SubPieces { get; set; }
    }

    public class RevisionDiffSubPieceDto
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public int Position { get; set; }
    }
}