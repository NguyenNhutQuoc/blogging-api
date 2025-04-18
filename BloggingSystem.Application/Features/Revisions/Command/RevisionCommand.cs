using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Domain.Events;
using BloggingSystem.Domain.Exceptions;
using BloggingSystem.Shared.DTOs;
using BloggingSystem.Shared.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Application.Features.Revisions.Command
{
    #region Restore Revision Command

    public class RestoreRevisionCommand : IRequest<PostDto>
    {
        public long RevisionId { get; set; }
    }

    public class RestoreRevisionCommandHandler : IRequestHandler<RestoreRevisionCommand, PostDto>
    {
        private readonly IRepository<Domain.Entities.Revision> _revisionRepository;
        private readonly IRepository<Domain.Entities.Post> _postRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDomainEventService _domainEventService;
        private readonly IMapper _mapper;
        private readonly ILogger<RestoreRevisionCommandHandler> _logger;

        public RestoreRevisionCommandHandler(
            IRepository<Domain.Entities.Revision> revisionRepository,
            IRepository<Domain.Entities.Post> postRepository,
            ICurrentUserService currentUserService,
            IDomainEventService domainEventService,
            IMapper mapper,
            ILogger<RestoreRevisionCommandHandler> logger)
        {
            _revisionRepository = revisionRepository;
            _postRepository = postRepository;
            _currentUserService = currentUserService;
            _domainEventService = domainEventService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PostDto> Handle(RestoreRevisionCommand request, CancellationToken cancellationToken)
        {
            // Get the revision to restore
            var revisionSpec = new RevisionByIdSpecification(request.RevisionId);
            var revision = await _revisionRepository
                .FirstOrDefaultAsync(revisionSpec, cancellationToken);

            if (revision == null)
                throw new NotFoundException("Revision", request.RevisionId);

            // Get the post
            var post = await _postRepository.GetByIdAsync(revision.PostId, cancellationToken);
            if (post == null)
                throw new NotFoundException("Post", revision.PostId);

            // Ensure the user has permission (author or admin)
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
                throw new UnauthorizedAccessException("User not authenticated");

            bool isAdmin = _currentUserService.IsInRole("Admin");
            if (post.AuthorId != userId && !isAdmin)
                throw new ForbiddenException("You do not have permission to restore this revision");

            // Save current content as a new revision before restoring
            var latestRevisionNumber = new MaxRevisionNumberByPostSpecification(post.Id);
            var latestRevision =
                await _revisionRepository.FirstOrDefaultAsync(latestRevisionNumber, cancellationToken);
            if (latestRevision == null)
                throw new NotFoundException("Latest revision not found");
            // Increment revision number for new revision
            int latestRevisionNumberValue = latestRevision.RevisionNumber;
            int newRevisionNumber = latestRevisionNumberValue + 1;

            // Create new revision with current content
            var newRevision = Revision.Create(
                post.Id,
                userId.Value,
                post.Content,
                newRevisionNumber);

            await _revisionRepository.AddAsync(newRevision, cancellationToken);

            // Store old content for event
            string oldContent = post.Content;
// Restore content from the selected revision
            post.Content = revision.Content;

            // Add domain event for revision restoration
            post.RestoreFromRevision(userId.Value, revision.Id, revision.RevisionNumber, oldContent, revision.Content);

            // Update post
            await _postRepository.UpdateAsync(post, cancellationToken);

            // Publish domain events
            await _domainEventService.PublishEventsAsync(post.DomainEvents);

            await _domainEventService.PublishEventsAsync(revision.DomainEvents);

            return _mapper.Map<PostDto>(post);
        }
    }

    #endregion
}