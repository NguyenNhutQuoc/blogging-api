using System.Threading.Tasks;
using BloggingSystem.Application.Features.SavedPosts.Commands;
using BloggingSystem.Application.Features.SavedPosts.Queries;
using BloggingSystem.Shared.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/saved-posts")]
    [Authorize]
    public class SavedPostsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SavedPostsController> _logger;

        public SavedPostsController(IMediator mediator, ILogger<SavedPostsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Save a post for later reading
        /// </summary>
        [HttpPost("{postId}")]
        [ProducesResponseType(typeof(SavedPostDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SavedPostDto>> SavePost(long postId)
        {
            var command = new SavePostCommand { PostId = postId };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Remove a post from saved posts
        /// </summary>
        [HttpDelete("{postId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UnsavePost(long postId)
        {
            var command = new UnsavePostCommand { PostId = postId };
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Check if a post is saved by the current user
        /// </summary>
        [HttpGet("check/{postId}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<bool>> CheckSavedPost(long postId)
        {
            var query = new CheckSavedPostQuery { PostId = postId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get all saved posts for the current user
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponseDto<PostSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedResponseDto<PostSummaryDto>>> GetSavedPosts(
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetSavedPostsQuery 
            { 
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get count of saved posts for the current user
        /// </summary>
        [HttpGet("count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<int>> GetSavedPostsCount()
        {
            var query = new GetSavedPostsCountQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Search through saved posts
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(PaginatedResponseDto<PostSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedResponseDto<PostSummaryDto>>> SearchSavedPosts(
            [FromQuery] string searchTerm,
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            var query = new SearchSavedPostsQuery 
            { 
                SearchTerm = searchTerm,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}