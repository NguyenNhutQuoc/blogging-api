using BloggingSystem.Domain.Entities;
using BloggingSystem.Domain.Commons;

namespace BloggingSystem.Domain.Events
{
    public class CreatedCommentEvent: DomainEvent {
        public long UserId {get;set;}
        public long PostId {get;set;}
        public string? Content {get;set;}

        public CreatedCommentEvent(long userId, long postId, string? content) {
            UserId = userId;
            PostId = postId;
            Content = content;
        }
    }

    public class UpdatedCommentEvent: DomainEvent {
        public long UserId {get;set;}
        public long PostId {get;set;}
        public string? Content {get;set;}
        public string? Status {get; set;}
        
        public UpdatedCommentEvent(long userId, long postId, string? content, string? status) {
            UserId = userId;
            PostId = postId;
            Content = content;
            Status = status;
        }
    }

    public class DeletedCommentEvent: DomainEvent {
        public long UserId {get;set;}
        public long PostId {get;set;}
        
        public DeletedCommentEvent(long userId, long postId) {
            UserId = userId;
            PostId = postId;
        }
    }
}