using System.Threading;
using System.Threading.Tasks;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Features.Comment;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Domain.Events;
using BloggingSystem.Domain.Exceptions;
using BloggingSystem.Shared.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Application.Commands
{
    #region Create Comment Command

    public class CreateCommentCommand : IRequest<CommentDto>
    {
        public long PostId { get; set; }
        public long UserId { get; set; }
        public long? ParentId { get; set; }
        public string Content { get; set; }
    }

    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, CommentDto>
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<CreateCommentCommandHandler> _logger;

        public CreateCommentCommandHandler(
            IRepository<Comment> commentRepository,
            IRepository<Post> postRepository,
            IRepository<User> userRepository,
            IDomainEventService domainEventService,
            ILogger<CreateCommentCommandHandler> logger)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _domainEventService = domainEventService;
            _logger = logger;
        }

        public async Task<CommentDto> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            // Validate post exists
            var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
            if (post == null)
                throw new DomainException("Post not found");

            // Validate user exists
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                throw new DomainException("User not found");

            // Validate post allows comments
            if (post.CommentStatus != "open")
                throw new DomainException("Comments are closed for this post");
            
            // Create the comment
            var comment = Comment.Create(request.PostId, request.UserId, request.Content);
            
            if (request.ParentId.HasValue)
            {
                var parentComment = await _commentRepository.GetByIdAsync(request.ParentId.Value, cancellationToken);
                if (parentComment == null)
                    throw new DomainException("Parent comment not found");

                if (parentComment.PostId != request.PostId)
                    throw new DomainException("Parent comment belongs to a different post");
                comment.SetParent(parentComment);
            }
            
            comment.UpdateStatus(CommentStatus.Approved);
            
            // Add domain event
            comment.AddDomainEvent(new CommentAddedEvent(comment.Id, comment.PostId, comment.UserId));

            // Save the comment
            await _commentRepository.AddAsync(comment, cancellationToken);

            // Publish domain events
            await _domainEventService.PublishEventsAsync(comment.DomainEvents);

            // Return DTO
            return new CommentDto
            {
                Id = comment.Id,
                PostId = comment.PostId,
                UserId = comment.UserId,
                ParentId = comment.ParentId,
                Content = comment.Content,
                Status = comment.Status == CommentStatus.Approved.ToString() ? "approved" : "pending",
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                User = new UserSummaryDto
                {
                    Id = user.Id,
                    Username = user.Username
                },
                Post = new PostSummaryDto
                {
                    Id = post.Id,
                    Title = post.Title,
                    Slug = post.Slug
                }
            };
        }
    }

    #endregion

    #region Update Comment Command

    public class UpdateCommentCommand : IRequest<CommentDto>
    {
        public long Id { get; set; }
        public long UserId { get; set; } // User performing the update
        public string Content { get; set; }
    }

    public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, CommentDto>
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<UpdateCommentCommandHandler> _logger;

        public UpdateCommentCommandHandler(
            IRepository<Comment> commentRepository,
            IRepository<User> userRepository,
            IDomainEventService domainEventService,
            ILogger<UpdateCommentCommandHandler> logger)
        {
            _commentRepository = commentRepository;
            _userRepository = userRepository;
            _domainEventService = domainEventService;
            _logger = logger;
        }

        public async Task<CommentDto> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            // Get the comment
            var comment = await _commentRepository.GetByIdAsync(request.Id, cancellationToken);
            if (comment == null)
                throw new DomainException("Comment not found");

            // Validate ownership (only the author can update their comment)
            if (comment.UserId != request.UserId)
                throw new DomainException("You can only edit your own comments");

            // Validate status (can't edit approved/rejected comments)
            if (comment.Status != "pending" && comment.Status != "approved")
                throw new DomainException("You cannot edit this comment in its current state");

            // Update the comment
            comment.Content = request.Content;

            // If comment was already approved, it may need re-approval
            if (comment.Status == "approved")
            {
                comment.Status = "pending";
                comment.AddDomainEvent(new CommentUpdatedEvent(comment.Id, comment.PostId, comment.UserId, CommentStatus.Approved));
            }

            // Save the comment
            await _commentRepository.UpdateAsync(comment, cancellationToken);

            // Publish domain events
            await _domainEventService.PublishEventsAsync(comment.DomainEvents);

            // Get user data
            var user = await _userRepository.GetByIdAsync(comment.UserId, cancellationToken);

            // Return DTO
            return new CommentDto
            {
                Id = comment.Id,
                PostId = comment.PostId,
                UserId = comment.UserId,
                ParentId = comment.ParentId,
                Content = comment.Content,
                Status = comment.Status,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                User = new UserSummaryDto
                {
                    Id = user.Id,
                    Username = user.Username
                }
            };
        }
    }

    #endregion

    #region Delete Comment Command

    public class DeleteCommentCommand : IRequest<bool>
    {
        public long Id { get; set; }
        public long UserId { get; set; } // User performing the delete
        public bool IsAdmin { get; set; } // Whether user has admin privileges
    }

    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, bool>
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<DeleteCommentCommandHandler> _logger;

        public DeleteCommentCommandHandler(
            IRepository<Comment> commentRepository,
            IDomainEventService domainEventService,
            ILogger<DeleteCommentCommandHandler> logger)
        {
            _commentRepository = commentRepository;
            _domainEventService = domainEventService;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            // Get the comment
            var comment = await _commentRepository.GetByIdAsync(request.Id, cancellationToken);
            if (comment == null)
                throw new DomainException("Comment not found");

            // Validate permissions (either admin or comment author)
            if (!request.IsAdmin && comment.UserId != request.UserId)
                throw new DomainException("You don't have permission to delete this comment");

            // Add domain event
            comment.AddDomainEvent(new CommentUpdatedEvent(comment.Id, comment.PostId, comment.UserId, CommentStatus.Trash));

            // Delete the comment
            await _commentRepository.DeleteAsync(comment, cancellationToken);

            // Publish domain events
            await _domainEventService.PublishEventsAsync(comment.DomainEvents);

            return true;
        }
    }

    #endregion

    #region Moderate Comment Command

    public class ModerateCommentCommand : IRequest<CommentDto>
    {
        public long Id { get; set; }
        public string Status { get; set; } // "approved", "rejected", "spam"
        public long ModeratorId { get; set; }
        public string ModeratorNote { get; set; }
    }

    public class ModerateCommentCommandHandler : IRequestHandler<ModerateCommentCommand, CommentDto>
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<ModerateCommentCommandHandler> _logger;

        public ModerateCommentCommandHandler(
            IRepository<Comment> commentRepository,
            IRepository<User> userRepository,
            IDomainEventService domainEventService,
            ILogger<ModerateCommentCommandHandler> logger)
        {
            _commentRepository = commentRepository;
            _userRepository = userRepository;
            _domainEventService = domainEventService;
            _logger = logger;
        }

        public async Task<CommentDto> Handle(ModerateCommentCommand request, CancellationToken cancellationToken)
        {
            // Get the comment
            var comment = await _commentRepository.GetByIdAsync(request.Id, cancellationToken);
            if (comment == null)
                throw new DomainException("Comment not found");

            // Validate status
            if (request.Status != "approved" && request.Status != "rejected" && request.Status != "spam")
                throw new DomainException("Invalid status value");

            // Save old status for event
            var oldStatus = comment.Status;

            // Update status
            comment.Status = request.Status;

            // Add appropriate domain event
            switch (request.Status)
            {
                case "approved":
                    comment.AddDomainEvent(new CommentUpdatedEvent(comment.Id, comment.PostId, comment.UserId, CommentStatus.Approved));
                    break;
                case "rejected":
                case "spam":
                    comment.AddDomainEvent(new CommentUpdatedEvent(comment.Id, comment.PostId, comment.UserId, CommentStatus.Spam));
                    break;
            }

            // Save the comment
            await _commentRepository.UpdateAsync(comment, cancellationToken);

            // Publish domain events
            await _domainEventService.PublishEventsAsync(comment.DomainEvents);

            // Get user data
            var user = await _userRepository.GetByIdAsync(comment.UserId, cancellationToken);

            // Return DTO
            return new CommentDto
            {
                Id = comment.Id,
                PostId = comment.PostId,
                UserId = comment.UserId,
                ParentId = comment.ParentId,
                Content = comment.Content,
                Status = comment.Status,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                User = new UserSummaryDto
                {
                    Id = user.Id,
                    Username = user.Username
                }
            };
        }
    }

    #endregion
}