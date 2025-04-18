using System;
using System.Threading;
using System.Threading.Tasks;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Domain.Events;
using BloggingSystem.Domain.Exceptions;
using BloggingSystem.Shared.Exceptions;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Application.Features.Post.Command
{
    #region Increment Post Views Command

    public class IncrementPostViewsCommand : IRequest<bool>
    {
        public string Slug { get; set; }
        public long PostId { get; set; }
        public string VisitorHash { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
    }

    public class IncrementPostViewsCommandHandler : IRequestHandler<IncrementPostViewsCommand, bool>
    {
        private readonly IRepository<Domain.Entities.Post> _postRepository;
        private readonly IMemoryCache _cache;
        private readonly IDomainEventService _domainEventService;
        private readonly IDateTime _dateTime;
        private readonly ILogger<IncrementPostViewsCommandHandler> _logger;

        // Cache duration for preventing duplicate views (in minutes)
        private const int CacheDurationMinutes = 30;

        public IncrementPostViewsCommandHandler(
            IRepository<Domain.Entities.Post> postRepository,
            IMemoryCache cache,
            IDomainEventService domainEventService,
            IDateTime dateTime,
            ILogger<IncrementPostViewsCommandHandler> logger)
        {
            _postRepository = postRepository;
            _cache = cache;
            _domainEventService = domainEventService;
            _dateTime = dateTime;
            _logger = logger;
        }

        public async Task<bool> Handle(IncrementPostViewsCommand request, CancellationToken cancellationToken)
        {
            // Check if already viewed within time window
            var cacheKey = request.PostId > 0 ? $"PostView_{request.PostId}_{request.VisitorHash}" : $"PostView_{request.Slug}_{request.VisitorHash}";
            if (_cache.TryGetValue(cacheKey, out _))
            {
                // Already viewed recently, don't count again
                return false;
            }
            
            var spec = new PostBySlugSpecification(request.Slug);

            // Get post
            var post = request.PostId > 0 
                ? await _postRepository.GetByIdAsync(request.PostId, cancellationToken)
                : await _postRepository.FirstOrDefaultAsync(spec, cancellationToken);
            if (post == null)
                throw new NotFoundException("Post", request.PostId > 0 ? request.PostId : request.Slug);

            // Check if post is published
            if (post.Status != "published")
                return false;

            // Set cache to prevent duplicate views
            _cache.Set(cacheKey, true, TimeSpan.FromMinutes(CacheDurationMinutes));

            // Increment post view count
             post.IncreaseView(request.IpAddress, request.UserAgent);
            // Update post
            await _postRepository.UpdateAsync(post, cancellationToken);

            // Publish domain events
            await _domainEventService.PublishEventsAsync(post.DomainEvents);

            return true;
        }
    }

    #endregion
}