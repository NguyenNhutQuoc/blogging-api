
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Features.Post.Command;
using BloggingSystem.Application.Queries;
using BloggingSystem.Shared.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloggingSystem.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/posts")]
    public class PostsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PostsController> _logger;
        private readonly ICurrentUserService _currentUserService;

        public PostsController(IMediator mediator, ILogger<PostsController> logger,
            ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Get all posts with pagination
        /// </summary>
        [HttpGet]
        [Authorize("Permission:post.read-all")]
        [ProducesResponseType(typeof(PaginatedResponseDto<PostSummaryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponseDto<PostSummaryDto>>> GetPosts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetPostsQuery()
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get published posts with pagination
        /// </summary>
        [HttpGet("published")]
        [ProducesResponseType(typeof(PaginatedResponseDto<PostSummaryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponseDto<PostSummaryDto>>> GetPublishedPosts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetPublishedPostsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(ResponseDto<PaginatedResponseDto<PostSummaryDto>>.SuccessResponse(result, "Posts retrieved successfully"));


        }

        /// <summary>
        /// Get a post by ID
        /// </summary>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostDto>> GetPostById(long id)
        {
            var query = new GetPostByIdQuery { Id = id };
            var post = await _mediator.Send(query);

            // Check If-None-Match header
            var requestETag = Request.Headers.IfNoneMatch.ToString();
            if (!string.IsNullOrEmpty(requestETag) && requestETag == post.ETag)
            {
                // If ETag matches, return 304 Not Modified
                return StatusCode(StatusCodes.Status304NotModified);
            }

            // Check If-Modified-Since header
            if (Request.Headers.IfModifiedSince.Count > 0)
            {
                var ifModifiedSince = Request.Headers.IfModifiedSince[0];
                if (DateTime.TryParse(ifModifiedSince, out var modifiedSince))
                {
                    if (post.UpdatedAt <= modifiedSince)
                    {
                        return StatusCode(StatusCodes.Status304NotModified);
                    }
                }
            }
    
            // Return the post data
            return Ok(post);

        }

        /// <summary>
        /// Get a post by slug
        /// </summary>
        [HttpGet("by-slug/{slug}")]
        [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostDto>> GetPostBySlug(string slug)
        {
            var query = new GetPostBySlugQuery { Slug = slug };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get posts by author
        /// </summary>
        [HttpGet("by-author/{authorId:long}")]
        [ProducesResponseType(typeof(PaginatedResponseDto<PostSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaginatedResponseDto<PostSummaryDto>>> GetPostsByAuthor(
            long authorId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetPostsPublishedByAuthorQuery
            {
                AuthorId = authorId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get posts by category
        /// </summary>
        [HttpGet("by-category/{categoryId:long}")]
        [ProducesResponseType(typeof(PaginatedResponseDto<PostSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaginatedResponseDto<PostSummaryDto>>> GetPostsByCategory(
            long categoryId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetPostsByCategoryQuery
            {
                CategoryId = categoryId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get posts by tag
        /// </summary>
        [HttpGet("by-tag/{tagId:long}")]
        [ProducesResponseType(typeof(PaginatedResponseDto<PostSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaginatedResponseDto<PostSummaryDto>>> GetPostsByTag(
            long tagId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetPostsByTagQuery
            {
                TagId = tagId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Search posts
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(PaginatedResponseDto<PostSummaryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponseDto<PostSummaryDto>>> SearchPosts(
            [FromQuery] string searchTerm,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new SearchPostsPublishedQuery
            {
                SearchTerm = searchTerm,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get my posts
        /// </summary>
        [HttpGet("my-posts")]
        [Authorize]
        [ProducesResponseType(typeof(PaginatedResponseDto<PostSummaryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponseDto<PostSummaryDto>>> GetMyPosts(
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = _currentUserService.UserId;
            var query = new GetPostsByAuthorQuery()
            {
                AuthorId = userId ?? 0,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Search my posts
        /// </summary>
        [HttpGet("my-posts/search")]
        [Authorize]
        [ProducesResponseType(typeof(PaginatedResponseDto<PostSummaryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponseDto<PostSummaryDto>>> SearchMyPosts(
            [FromQuery] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = _currentUserService.UserId;
            var query = new SearchPostsByAuthorQuery()
            {
                SearchTerm = searchTerm,
                AuthorId = userId?? 0,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

    /// <summary>
        /// Create a new post
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "Permission:post.create")]
        [ProducesResponseType(typeof(PostDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetPostById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Update an existing post
        /// </summary>
        [HttpPut("{id:long}")]
        [Authorize(Policy = "Permission:post.edit")]
        [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostDto>> UpdatePost(long id, [FromBody] UpdatePostCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID in URL does not match ID in command");

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Delete a post
        /// </summary>
        [HttpDelete("{id:long}")]
        [Authorize(Policy = "Permission:post.delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePost(long id)
        {
            var command = new DeletePostCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Publish a post
        /// </summary>
        [HttpPost("{id:long}/publish")]
        [Authorize(Policy = "Permission:post.publish")]
        [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostDto>> PublishPost(long id)
        {
            var command = new PublishPostCommand { Id = id };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        
        
        [HttpPost("{id:long}/unpublish")]
        [Authorize(Policy = "Permission:post.unpublish")]
        [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnpublishPost(long id)
        {
            var command = new UnpublishPostCommand { Id = id };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Archive a post
        /// </summary>
        [HttpPost("{id:long}/archive")]
        [Authorize(Policy = "Permission:post.archive")]
        [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostDto>> ArchivePost(long id)
        {
            var command = new ArchivePostCommand { Id = id };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }

}