using AutoMapper;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Features.PostCategory;
using BloggingSystem.Application.Features.PostTag;
using BloggingSystem.Application.Features.Revisions;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Domain.Events;
using BloggingSystem.Domain.Exceptions;
using BloggingSystem.Shared.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Application.Features.Post.Command
{
    #region Create Post Command

    public class CreatePostCommand : IRequest<PostDto>
    {
        public long AuthorId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Excerpt { get; set; }
        public int? MediaId { get; set; }
        public List<long> CategoryIds { get; set; } = new List<long>();
        public List<long> TagIds { get; set; } = new List<long>();
    }

    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, PostDto>
    {
        private readonly IRepository<Domain.Entities.Post> _postRepository;
        private readonly IRepository<Domain.Entities.User> _userRepository;
        private readonly IRepository<Domain.Entities.Category> _categoryRepository;
        private readonly IRepository<Domain.Entities.Tag> _tagRepository;
        private readonly IRepository<Medium> _mediaRepository;
        private readonly IRepository<Domain.Entities.PostCategory> _postCategoryRepository;
        private readonly IRepository<Domain.Entities.PostTag> _postTagRepository;
        private readonly IRepository<PostMedium> _postMediumRepository;
        private readonly IDomainEventService _domainEventService;
        private readonly ISlugService _slugService;
        private readonly ILogger<CreatePostCommandHandler> _logger;

        public CreatePostCommandHandler(
            IRepository<Domain.Entities.Post> postRepository,
            IRepository<Domain.Entities.User> userRepository,
            IRepository<Domain.Entities.Category> categoryRepository,
            IRepository<Domain.Entities.Tag> tagRepository,
            IRepository<Domain.Entities.PostCategory> postCategoryRepository,
            IRepository<Domain.Entities.PostTag> postTagRepository,
            IRepository<Medium> mediaRepository,
            IRepository<PostMedium> postMediumRepository,
            IDomainEventService domainEventService,
            ISlugService slugService,
            ILogger<CreatePostCommandHandler> logger)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _postCategoryRepository = postCategoryRepository;
            _postTagRepository = postTagRepository;
            _mediaRepository = mediaRepository;
            _postMediumRepository = postMediumRepository;
            _domainEventService = domainEventService;
            _slugService = slugService;
            _logger = logger;
        }

        public async Task<PostDto> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            var author = await _userRepository.GetByIdAsync(request.AuthorId, cancellationToken);
            if (author == null)
                throw new DomainException("Author not found");

            // Generate slug from title
            var baseSlug = _slugService.GenerateSlug(request.Title);
            var slug = baseSlug;

            // Create post
            var post = Domain.Entities.Post.CreatePost(slug: slug, content: request.Content, title: request.Title,
                authorId: request.AuthorId, excerpt: request.Excerpt ?? string.Empty);

            if (request.MediaId != null)
            {
                // Check if media exists
                var media = await _mediaRepository.GetByIdAsync(request.MediaId.Value, cancellationToken);
                if (media == null)
                    throw new DomainException("Media not found");

                // Set featured image URL
                post.FeaturedImageUrl = media.FilePath;
            }

            // Set published date if the post is being published directly
            if (post.Status == "published")
            {
                post.Publish();
            }

            // Save post
            await _postRepository.AddAsync(post, cancellationToken);
            
            if (request.MediaId != null)
            {
                // Check if media exists
                var media = await _mediaRepository.GetByIdAsync(request.MediaId.Value, cancellationToken);
                if (media == null)
                    throw new DomainException("Media not found");
                // Create post medium
                var postMedium = PostMedium.Create(post.Id, media.Id, 0);
                
                await _postMediumRepository.AddAsync(postMedium, cancellationToken);
            }

            // Add categories
            foreach (var categoryId in request.CategoryIds)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
                if (category != null)
                {
                    var postCategory = Domain.Entities.PostCategory.Create(post.Id, categoryId);
                    await _postCategoryRepository.AddAsync(postCategory, cancellationToken);
                }
            }

            // Add tags
            foreach (var tagId in request.TagIds)
            {
                var tag = await _tagRepository.GetByIdAsync(tagId, cancellationToken);
                if (tag != null)
                {
                    var postTag = Domain.Entities.PostTag.Create(post.Id, tagId);
                    await _postTagRepository.AddAsync(postTag, cancellationToken);
                }
            }

            // Publish domain events
            await _domainEventService.PublishEventsAsync(post.DomainEvents);

            // Return DTO
            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Slug = post.Slug,
                Excerpt = post.Excerpt,
                Content = post.Content,
                FeaturedImageUrl = post.FeaturedImageUrl,
                Status = post.Status,
                CommentStatus = post.CommentStatus
            };
        }
    }

    #endregion

    #region Update Post Command

    public class UpdatePostCommand : IRequest<PostDto>
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? Excerpt { get; set; }
        public string? FeaturedImageUrl { get; set; }
        public string Status { get; set; }
        public string CommentStatus { get; set; }
        public List<long> CategoryIds { get; set; } = new List<long>();
        public List<long> TagIds { get; set; } = new List<long>();
    }

    public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, PostDto>
    {
        private readonly IRepository<Domain.Entities.Post> _postRepository;
        private readonly IRepository<Domain.Entities.User> _userRepository;
        private readonly IRepository<Domain.Entities.Category> _categoryRepository;
        private readonly IRepository<Domain.Entities.Tag> _tagRepository;
        private readonly IRepository<Domain.Entities.PostCategory> _postCategoryRepository;
        private readonly IRepository<Domain.Entities.PostTag> _postTagRepository;
        private readonly IRepository<Revision> _revisionRepository;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<UpdatePostCommandHandler> _logger;

        public UpdatePostCommandHandler(
            IRepository<Domain.Entities.Post> postRepository,
            IRepository<Domain.Entities.User> userRepository,
            IRepository<Domain.Entities.Category> categoryRepository,
            IRepository<Domain.Entities.Tag> tagRepository,
            IRepository<Domain.Entities.PostCategory> postCategoryRepository,
            IRepository<Domain.Entities.PostTag> postTagRepository,
            IRepository<Revision> revisionRepository,
            IDomainEventService domainEventService,
            ILogger<UpdatePostCommandHandler> logger)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _postCategoryRepository = postCategoryRepository;
            _postTagRepository = postTagRepository;
            _revisionRepository = revisionRepository;
            _domainEventService = domainEventService;
            _logger = logger;
        }

        public async Task<PostDto> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.Id, cancellationToken);
            if (post == null)
                throw new DomainException("Post not found");
            
            // Check iof the post is already published, cannot be  updated
            if (post.Status == "published")
                throw new DomainException("Post is already published and cannot be updated");

            var wasPublished = post.Status == "published";
            var isPublishedNow = request.Status == "published";
            
            // Lưu nội dung cũ trước khi update
            string oldContent = post.Content;

            // Lấy revision number cao nhất
            var latestRevisionSpec = new MaxRevisionNumberByPostSpecification(post.Id);
            var latestRevision = await _revisionRepository.FirstOrDefaultAsync(latestRevisionSpec, cancellationToken);
            
            int latestRevisionNumber = latestRevision?.RevisionNumber ?? 0;
            int newRevisionNumber = latestRevisionNumber + 1;
            
            // Create new revision
            var revisionPost = Revision.Create(
                postId: post.Id,
                userId: post.AuthorId,
                content: oldContent,
                revisionNumber: newRevisionNumber
            );
            await _revisionRepository.AddAsync(revisionPost, cancellationToken);

            // Update post
            post.Update(request.Title, post.Slug, request.Title, request.Excerpt, request.FeaturedImageUrl, request.Status, request.CommentStatus);
            // If post is being published for the first time, set published date
            if (!wasPublished && isPublishedNow)
            {
                post.Publish();
            }

            // Update post
            await _postRepository.UpdateAsync(post, cancellationToken);

            // Update categories - get existing categories
            var postCategorySpec = new PostCategoryByPostIdSpecification(post.Id);
            var existingPostCategories = await _postCategoryRepository.ListAsync(postCategorySpec, cancellationToken);

            // Remove categories that are no longer associated
            // TODO: Optimize this
            foreach (var postCategory in existingPostCategories)
            {
                if (!request.CategoryIds.Contains(postCategory.CategoryId))
                {
                    await _postCategoryRepository.DeleteAsync(postCategory, cancellationToken);
                }
            }

            // Add new categories
            // TODO: Optimize this
            var existingCategoryIds = existingPostCategories.Select(pc => pc.CategoryId).ToList();
            foreach (var categoryId in request.CategoryIds)
            {
                if (!existingCategoryIds.Contains(categoryId))
                {
                    var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
                    if (category != null)
                    {
                        var postCategory = new Domain.Entities.PostCategory(post.Id, categoryId);
                        await _postCategoryRepository.AddAsync(postCategory, cancellationToken);
                    }
                }
            }

            // Update tags - get existing tags
            // TODO: Optimize this
            var postTagSpec = new PostTagByPostIdSpecification(post.Id);
            var existingPostTags = await _postTagRepository.ListAsync(postTagSpec, cancellationToken);

            // Remove tags that are no longer associated
            foreach (var postTag in existingPostTags)
            {
                if (!request.TagIds.Contains(postTag.TagId))
                {
                    await _postTagRepository.DeleteAsync(postTag, cancellationToken);
                }
            }

            // Add new tags
            var existingTagIds = existingPostTags.Select(pt => pt.TagId).ToList();
            // TODO: Optimize this
            foreach (var tagId in request.TagIds)
            {
                if (!existingTagIds.Contains(tagId))
                {
                    var tag = await _tagRepository.GetByIdAsync(tagId, cancellationToken);
                    if (tag != null)
                    {
                        var postTag = new Domain.Entities.PostTag(post.Id, tagId);
                        await _postTagRepository.AddAsync(postTag, cancellationToken);
                    }
                }
            }

            // Publish domain events
            await _domainEventService.PublishEventsAsync(post.DomainEvents);

            // Get author for DTO
            var author = await _userRepository.GetByIdAsync(post.AuthorId, cancellationToken);

            // Return DTO
            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Slug = post.Slug,
                Excerpt = post.Excerpt,
                Content = post.Content,
                FeaturedImageUrl = post.FeaturedImageUrl,
                Status = post.Status,
                CommentStatus = post.CommentStatus,
                CreatedAt = post.CreatedAt,
                PublishedAt = post.PublishedAt,
                UpdatedAt = post.UpdatedAt,
            };
        }
    }

    #endregion

    #region Delete Post Command

    public class DeletePostCommand : IRequest<bool>
    {
        public long Id { get; set; }
    }

    public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, bool>
    {
        private readonly IRepository<Domain.Entities.Post> _postRepository;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<DeletePostCommandHandler> _logger;

        public DeletePostCommandHandler(
            IRepository<Domain.Entities.Post> postRepository,
            IDomainEventService domainEventService,
            ILogger<DeletePostCommandHandler> logger)
        {
            _postRepository = postRepository;
            _domainEventService = domainEventService;
            _logger = logger;
        }

        public async Task<bool> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.Id, cancellationToken);
            if (post == null)
                throw new DomainException("Post not found");

            post.Trash();

            // Delete post
            await _postRepository.UpdateAsync(post, cancellationToken);

            // Publish domain events
            await _domainEventService.PublishEventsAsync(post.DomainEvents);

            return true;
        }
    }

    #endregion

    #region Publish Post Command

    public class PublishPostCommand : IRequest<PostDto>
    {
        public long Id { get; set; }
    }

    public class PublishPostCommandHandler : IRequestHandler<PublishPostCommand, PostDto>
    {
        private readonly IRepository<Domain.Entities.Post> _postRepository;
        private readonly IRepository<Domain.Entities.User> _userRepository;
        private readonly IDomainEventService _domainEventService;
        private readonly IMapper _mapper;
        private readonly ILogger<PublishPostCommandHandler> _logger;

        public PublishPostCommandHandler(
            IRepository<Domain.Entities.Post> postRepository,
            IRepository<Domain.Entities.User> userRepository,
            IDomainEventService domainEventService,
            IMapper mapper,
            ILogger<PublishPostCommandHandler> logger)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _domainEventService = domainEventService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PostDto> Handle(PublishPostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.Id, cancellationToken);
            if (post == null)
                throw new DomainException("Post not found");

            if (post.Status == "published")
                throw new DomainException("Post is already published");

            // Update status and set publish date
            post.Publish();

            // Update post
            await _postRepository.UpdateAsync(post, cancellationToken);

            // Publish domain events
            await _domainEventService.PublishEventsAsync(post.DomainEvents);

            // Get author for DTO
            var author = await _userRepository.GetByIdAsync(post.AuthorId, cancellationToken);

            // Return DTO
            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Slug = post.Slug,
                Excerpt = post.Excerpt,
                Content = post.Content,
                FeaturedImageUrl = post.FeaturedImageUrl,
                Status = post.Status,
                CommentStatus = post.CommentStatus,
                CreatedAt = post.CreatedAt,
                PublishedAt = post.PublishedAt,
                UpdatedAt = post.UpdatedAt,
                Author = _mapper.Map<AuthorDto>(author)
            };
        }
    }

    #endregion
    
    #region Unpublish Post Command
    public class UnpublishPostCommand : IRequest<PostDto>
    {
        public long Id { get; set; }
    }
    public class UnpublishPostCommandHandler : IRequestHandler<UnpublishPostCommand, PostDto>
    {
        private readonly IRepository<Domain.Entities.Post> _postRepository;
        private readonly IRepository<Domain.Entities.User> _userRepository;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<UnpublishPostCommandHandler> _logger;
        private readonly IMapper _mapper;

        public UnpublishPostCommandHandler(
            IRepository<Domain.Entities.Post> postRepository,
            IRepository<Domain.Entities.User> userRepository,
            IDomainEventService domainEventService,
            IMapper mapper,
            ILogger<UnpublishPostCommandHandler> logger)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _domainEventService = domainEventService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PostDto> Handle(UnpublishPostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.Id, cancellationToken);
            if (post == null)
                throw new DomainException("Post not found");

            if (post.Status != "published")
                throw new DomainException("Post is not published");

            // Update status
            post.UnPublish();

            // Update post
            await _postRepository.UpdateAsync(post, cancellationToken);

            // Publish domain events
            await _domainEventService.PublishEventsAsync(post.DomainEvents);

            // Get author for DTO
            var author = await _userRepository.GetByIdAsync(post.AuthorId, cancellationToken);

            // Return DTO
            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Slug = post.Slug,
                Excerpt = post.Excerpt,
                Content = post.Content,
                FeaturedImageUrl = post.FeaturedImageUrl,
                Status = post.Status,
                CommentStatus = post.CommentStatus,
                CreatedAt = post.CreatedAt,
                PublishedAt = post.PublishedAt,
                UpdatedAt = post.UpdatedAt,
                Author = _mapper.Map<AuthorDto>(author)
            };
        }
    }
    #endregion

    #region Archive Post Command

    public class ArchivePostCommand : IRequest<PostDto>
    {
        public long Id { get; set; }
    }

    public class ArchivePostCommandHandler : IRequestHandler<ArchivePostCommand, PostDto>
    {
        private readonly IRepository<Domain.Entities.Post> _postRepository;
        private readonly IRepository<Domain.Entities.User> _userRepository;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<ArchivePostCommandHandler> _logger;

        public ArchivePostCommandHandler(
            IRepository<Domain.Entities.Post> postRepository,
            IRepository<Domain.Entities.User> userRepository,
            IDomainEventService domainEventService,
            ILogger<ArchivePostCommandHandler> logger)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _domainEventService = domainEventService;
            _logger = logger;
        }

        public async Task<PostDto> Handle(ArchivePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.Id, cancellationToken);
            if (post == null)
                throw new DomainException("Post not found");

            if (post.Status == PostStatus.Trash.ToString())
                throw new DomainException("Post is already archived");

            post.Trash();

            // Update post
            await _postRepository.UpdateAsync(post, cancellationToken);

            // Publish domain events
            await _domainEventService.PublishEventsAsync(post.DomainEvents);

            // Get author for DTO
            var author = await _userRepository.GetByIdAsync(post.AuthorId, cancellationToken);

            // Return DTO
            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Slug = post.Slug,
                Excerpt = post.Excerpt,
                Content = post.Content,
                FeaturedImageUrl = post.FeaturedImageUrl,
                Status = post.Status,
                CommentStatus = post.CommentStatus,
                CreatedAt = post.CreatedAt,
                PublishedAt = post.PublishedAt,
                UpdatedAt = post.UpdatedAt,
            };
        }
    }

    #endregion
    

}