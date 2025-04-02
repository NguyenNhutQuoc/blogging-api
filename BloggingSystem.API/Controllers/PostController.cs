
using BloggingSystem.Application.Features.Post.Command;
using BloggingSystem.Application.Queries;
using BloggingSystem.Shared.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloggingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PostsController> _logger;

        public PostsController(IMediator mediator, ILogger<PostsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get all posts with pagination
        /// </summary>
        [HttpGet]
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
            return Ok(result);
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
            var result = await _mediator.Send(query);
            return Ok(result);
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
            var query = new GetPostsByAuthorQuery
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
            var query = new SearchPostsQuery
            {
                SearchTerm = searchTerm,
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
        [Authorize(Policy = "CreatePost")]
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
        [Authorize(Policy = "UpdatePost")]
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
        [Authorize(Policy = "DeletePost")]
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
        [Authorize(Policy = "PublishPost")]
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

        /// <summary>
        /// Archive a post
        /// </summary>
        [HttpPost("{id:long}/archive")]
        [Authorize(Policy = "ArchivePost")]
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