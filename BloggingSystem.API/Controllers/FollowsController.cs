using System.Threading.Tasks;
using BloggingSystem.Application.Features.Follows.Commands;
using BloggingSystem.Application.Features.Follows.Queries;
using BloggingSystem.Shared.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/follows")]
    [Authorize]
    public class FollowsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<FollowsController> _logger;

        public FollowsController(IMediator mediator, ILogger<FollowsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Follow a user
        /// </summary>
        [HttpPost("{userId}")]
        [ProducesResponseType(typeof(FollowDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FollowDto>> FollowUser(long userId)
        {
            var command = new FollowUserCommand { FollowingId = userId };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Unfollow a user
        /// </summary>
        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UnfollowUser(long userId)
        {
            var command = new UnfollowUserCommand { FollowingId = userId };
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Check if the current user follows a specific user
        /// </summary>
        [HttpGet("check/{userId}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<bool>> CheckFollow(long userId)
        {
            var query = new CheckFollowQuery { FollowingId = userId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get users that the current user follows
        /// </summary>
        [HttpGet("following")]
        [ProducesResponseType(typeof(PaginatedResponseDto<UserSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedResponseDto<UserSummaryDto>>> GetFollowing(
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetFollowingQuery { PageNumber = pageNumber, PageSize = pageSize };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get users who follow the current user
        /// </summary>
        [HttpGet("followers")]
        [ProducesResponseType(typeof(PaginatedResponseDto<UserSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedResponseDto<UserSummaryDto>>> GetFollowers(
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetFollowersQuery { PageNumber = pageNumber, PageSize = pageSize };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get followers of a specific user
        /// </summary>
        [HttpGet("followers/{userId}")]
        [ProducesResponseType(typeof(PaginatedResponseDto<UserSummaryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponseDto<UserSummaryDto>>> GetUserFollowers(
            long userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetUserFollowersQuery 
            { 
                UserId = userId, 
                PageNumber = pageNumber, 
                PageSize = pageSize 
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get users followed by a specific user
        /// </summary>
        [HttpGet("following/{userId}")]
        [ProducesResponseType(typeof(PaginatedResponseDto<UserSummaryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponseDto<UserSummaryDto>>> GetUserFollowing(
            long userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetUserFollowingQuery 
            { 
                UserId = userId, 
                PageNumber = pageNumber, 
                PageSize = pageSize 
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}