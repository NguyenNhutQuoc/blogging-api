using BloggingSystem.Domain.Entities;
using BloggingSystem.Domain.Commons;
using System.Data.Common;

namespace BloggingSystem.Domain.Events
{
    public class CreatedNewRevisionEvent: DomainEvent {
        public long PostId {get;}
        public long UserId {get;} 
        public long RevisionNumber {get;}
        public string OldContent {get;} = null!;
        public string RevisionContent {get;} = null!;

        public CreatedNewRevisionEvent(long postId, long userId, int revisionNumber, string oldContent, string revisionContent) {
            PostId = postId;
            UserId = userId;
            RevisionNumber = revisionNumber;
            OldContent = oldContent;
            RevisionContent = revisionContent;
        }
    }
}