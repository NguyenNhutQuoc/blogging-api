using System.Collections.Generic;
using System.Threading.Tasks;
using BloggingSystem.Application.Commands;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Features.Comment;
using BloggingSystem.Application.Queries;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Shared.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.API.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<CommentsController> _logger;

        public CommentsController(IMediator mediator, ICurrentUserService currentUserService, ILogger<CommentsController> logger)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        /// <summary>
        /// Get all comments with pagination
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponseDto<CommentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginatedResponseDto<CommentDto>>> GetComments(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetCommentsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get a comment by ID
        /// </summary>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(CommentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CommentDto>> GetCommentById(long id)
        {
            var query = new GetCommentByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get comments for a specific post
        /// </summary>
        [HttpGet("by-post/{postId:long}")]
        [ProducesResponseType(typeof(PaginatedResponseDto<CommentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaginatedResponseDto<CommentDto>>> GetCommentsByPost(
            long postId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool includeReplies = true)
        {
            var query = new GetCommentsByPostQuery
            {
                PostId = postId,
                PageNumber = pageNumber,
                PageSize = pageSize,
                IncludeReplies = includeReplies
            };
            
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get comments by a specific user
        /// </summary>
        [HttpGet("by-user/{userId:long}")]
        [Authorize]
        [ProducesResponseType(typeof(PaginatedResponseDto<CommentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaginatedResponseDto<CommentDto>>> GetCommentsByUser(
            long userId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            // Check if the user is requesting their own comments or is an admin
            if (!_currentUserService.UserId.HasValue)
                return Unauthorized();

            var currentUserId = _currentUserService.UserId.Value;
            var isAdmin = _currentUserService.IsInRole("Admin");
            
            if (currentUserId != userId && !isAdmin)
                return Forbid();
            
            var query = new GetCommentsByUserQuery
            {
                UserId = userId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get comments by status (for moderation)
        /// </summary>
        [HttpGet("by-status/{status}")]
        [Authorize(Policy = "ModerateComments")]
        [ProducesResponseType(typeof(PaginatedResponseDto<CommentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginatedResponseDto<CommentDto>>> GetCommentsByStatus(
            string status,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            // Validate status
            if (status != "pending" && status != "approved" && status != "rejected" && status != "spam")
                return BadRequest("Invalid status value");
            
            var query = new GetCommentsByStatusQuery
            {
                Status = status,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get replies to a specific comment
        /// </summary>
        [HttpGet("{commentId:long}/replies")]
        [ProducesResponseType(typeof(List<CommentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<CommentDto>>> GetCommentReplies(long commentId)
        {
            var query = new GetCommentRepliesQuery { CommentId = commentId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Search comments
        /// </summary>
        [HttpGet("search")]
        [Authorize(Policy = "ManageComments")]
        [ProducesResponseType(typeof(PaginatedResponseDto<CommentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginatedResponseDto<CommentDto>>> SearchComments(
            [FromQuery] string searchTerm,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new SearchCommentsQuery
            {
                SearchTerm = searchTerm,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Create a new comment
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(CommentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CommentDto>> CreateComment([FromBody] CreateCommentDto dto)
        {
            
            var command = new CreateCommentCommand()
            {
                PostId = dto.PostId,
                ParentId = dto.ParentId,
                Content = dto.Content
            };
            
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetCommentById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Update an existing comment
        /// </summary>
        [HttpPut("{id:long}")]
        [Authorize]
        [ProducesResponseType(typeof(CommentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CommentDto>> UpdateComment(long id, [FromBody] UpdateCommentDto dto)
        {
            // Get the current user's ID from claims
            if (!_currentUserService.UserId.HasValue)
                return Unauthorized();

            var userId = _currentUserService.UserId.Value;
            
            var command = new UpdateCommentCommand
            {
                Id = id,
                UserId = userId,
                Content = dto.Content
            };
            
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Delete a comment
        /// </summary>
        [HttpDelete("{id:long}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComment(long id)
        {
            // Get the current moderator's ID from claims
            if (!_currentUserService.UserId.HasValue)
                return Unauthorized();
            // Get the current user's ID from claims
            var userId = _currentUserService.UserId.Value;
            
            // Check if user is admin
            var isAdmin = _currentUserService.IsInRole("Admin");
            
            var command = new DeleteCommentCommand
            {
                Id = id,
                UserId = userId,
                IsAdmin = isAdmin
            };
            
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Moderate a comment (approve, reject, mark as spam)
        /// </summary>
        [HttpPut("{id:long}/moderate")]
        [Authorize(Policy = "ModerateComments")]
        [ProducesResponseType(typeof(CommentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CommentDto>> ModerateComment(long id, [FromBody] ModerateCommentDto dto)
        {
            // Get the current moderator's ID from claims
            if (!_currentUserService.UserId.HasValue)
                return Unauthorized();

            var moderatorId = _currentUserService.UserId.Value;
            if (moderatorId == 0)
                return Unauthorized();
            
            // Validate status
            if (dto.Status != "approved" && dto.Status != "rejected" && dto.Status != "spam")
                return BadRequest("Invalid status value");
            
            var command = new ModerateCommentCommand
            {
                Id = id,
                Status = Comment.MapStatus(dto.Status),
                ModeratorId = moderatorId,
                ModeratorNote = dto.ModeratorNote
            };
            
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Get comment counts by status
        /// </summary>
        [HttpGet("count")]
        [Authorize(Policy = "ModerateComments")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<int>> GetCommentCount([FromQuery] CommentStatus status)
        {
            var query = new GetCommentCountByStatusQuery
            {
                Status = status
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}