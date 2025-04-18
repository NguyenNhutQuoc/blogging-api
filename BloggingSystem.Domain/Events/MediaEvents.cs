using BloggingSystem.Domain.Commons;

namespace BloggingSystem.Domain.Events
{
    public class MediaUploadedEvent : DomainEvent
    {
        public long MediaId { get; }
        public long UserId { get; }
        public string FileName { get; }

        public MediaUploadedEvent(long mediaId, long userId, string fileName)
        {
            MediaId = mediaId;
            UserId = userId;
            FileName = fileName;
        }
    }

    public class MediaDeletedEvent : DomainEvent
    {
        public long MediaId { get; }
        public long UserId { get; }
        public string FileName { get; }

        public MediaDeletedEvent(long mediaId, long userId, string fileName)
        {
            MediaId = mediaId;
            UserId = userId;
            FileName = fileName;
        }
    }
}