using BloggingSystem.Application.Features.Likes.Commands;
using BloggingSystem.Application.Features.Likes.Queries;
using BloggingSystem.Shared.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloggingSystem.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/likes")]
    [Authorize]
    public class LikesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<LikesController> _logger;

        public LikesController(IMediator mediator, ILogger<LikesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Like a post
        /// </summary>
        [HttpPost("post/{postId}")]
        [ProducesResponseType(typeof(LikeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LikeDto>> LikePost(long postId)
        {
            var command = new LikeEntityCommand 
            { 
                EntityType = "post", 
                EntityId = postId 
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Unlike a post
        /// </summary>
        [HttpDelete("post/{postId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UnlikePost(long postId)
        {
            var command = new UnlikeEntityCommand 
            { 
                EntityType = "post", 
                EntityId = postId 
            };
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Like a comment
        /// </summary>
        [HttpPost("comment/{commentId}")]
        [ProducesResponseType(typeof(LikeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LikeDto>> LikeComment(long commentId)
        {
            var command = new LikeEntityCommand 
            { 
                EntityType = "comment", 
                EntityId = commentId 
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Unlike a comment
        /// </summary>
        [HttpDelete("comment/{commentId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UnlikeComment(long commentId)
        {
            var command = new UnlikeEntityCommand 
            { 
                EntityType = "comment", 
                EntityId = commentId 
            };
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Check if the current user likes a specific entity
        /// </summary>
        [HttpGet("check")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<bool>> CheckLike(
            [FromQuery] string entityType, 
            [FromQuery] long entityId)
        {
            var query = new CheckLikeQuery 
            { 
                EntityType = entityType, 
                EntityId = entityId 
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get users who liked a specific post
        /// </summary>
        [HttpGet("post/{postId}/users")]
        [ProducesResponseType(typeof(PaginatedResponseDto<UserSummaryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponseDto<UserSummaryDto>>> GetPostLikeUsers(
            long postId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetEntityLikeUsersQuery 
            { 
                EntityType = "post", 
                EntityId = postId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get users who liked a specific comment
        /// </summary>
        [HttpGet("comment/{commentId}/users")]
        [ProducesResponseType(typeof(PaginatedResponseDto<UserSummaryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponseDto<UserSummaryDto>>> GetCommentLikeUsers(
            long commentId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetEntityLikeUsersQuery 
            { 
                EntityType = "comment", 
                EntityId = commentId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get like count for a specific entity
        /// </summary>
        [HttpGet("count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetLikeCount(
            [FromQuery] string entityType, 
            [FromQuery] long entityId)
        {
            var query = new GetLikeCountQuery 
            { 
                EntityType = entityType, 
                EntityId = entityId 
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get posts liked by the current user
        /// </summary>
        [HttpGet("my/posts")]
        [ProducesResponseType(typeof(PaginatedResponseDto<PostSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedResponseDto<PostSummaryDto>>> GetMyLikedPosts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetUserLikedPostsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}