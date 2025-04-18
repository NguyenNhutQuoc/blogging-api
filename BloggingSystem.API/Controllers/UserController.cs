using BloggingSystem.Application.Commands;
using BloggingSystem.Application.Queries;
using BloggingSystem.Application.Users.Queries;
using BloggingSystem.Shared.DTOs;
using BloggingSystem.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GetUserRolesQuery = BloggingSystem.Application.Queries.GetUserRolesQuery;

namespace BloggingSystem.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")] 
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsersController> _logger;
        
        public UsersController(
            IMediator mediator,
            ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !long.TryParse(userId, out var userIdLong))
                return Unauthorized();

            try
            {
                var query = new GetCurrentUserQuery() { UserId = userIdLong };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred" });
            }
        }
        
        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(long id)
        {
            var query = new GetUserByIdQuery { UserId = id };
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }
        
        /// <summary>
        /// Get user by username
        /// </summary>
        [HttpGet("by-username/{username}")]
        public async Task<ActionResult<UserDto>> GetByUsername(string username)
        {
            var query = new GetUserByUsernameQuery { Username = username };
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }
        
        /// <summary>
        /// Get active users
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PaginatedResponseDto<UserSummaryDto>>> GetActiveUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetActiveUsersQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }
        
        /// <summary>
        /// Search users
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<PaginatedResponseDto<UserSummaryDto>>> Search(
            [FromQuery] string searchTerm,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new SearchUsersQuery
            {
                SearchTerm = searchTerm,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }
        
        /// <summary>
        /// Get users by role
        /// </summary>
        [HttpGet("by-role/{roleId}")]
        public async Task<ActionResult<PaginatedResponseDto<UserSummaryDto>>> GetUsersByRole(
            long roleId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetUsersByRoleQuery
            {
                RoleId = roleId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }
        
        /// <summary>
        /// Get user profile
        /// </summary>
        [HttpGet("{userId}/profile")]
        public async Task<ActionResult<UserProfileDto>> GetUserProfile(long userId)
        {
            var query = new GetUserProfileQuery { UserId = userId };
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }
        
        /// <summary>
        /// Get user roles
        /// </summary>
        [HttpGet("{userId}/roles")]
        public async Task<ActionResult<List<RoleDto>>> GetUserRoles(long userId)
        {
            var query = new GetUserRolesQuery { UserId = userId };
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }
        
        /// <summary>
        /// Register new user
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterUserCommand command)
        {
            var result = await _mediator.Send(command);
            
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        
        /// <summary>
        /// Update user profile
        /// </summary>
        [HttpPut("{userId}/profile")]
        [Authorize]
        public async Task<ActionResult<UserDto>> UpdateProfile(long userId, UpdateUserProfileCommand command)
        {
            if (userId != command.UserId)
            {
                return BadRequest("User ID in the URL does not match the User ID in the request body");
            }
            
            var result = await _mediator.Send(command);
            
            return Ok(result);
        }
        
        /// <summary>
        /// Change password
        /// </summary>
        [HttpPost("{userId}/change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword(long userId, ChangePasswordCommand command)
        {
            if (userId != command.UserId)
            {
                return BadRequest("User ID in the URL does not match the User ID in the request body");
            }
            
            await _mediator.Send(command);
            
            return NoContent();
        }
        
        /// <summary>
        /// Activate user
        /// </summary>
        [HttpPost("{userId}/activate")]
        [Authorize]
        public async Task<ActionResult> Activate(long userId)
        {
            var command = new ActivateUserCommand { UserId = userId };
            await _mediator.Send(command);
            
            return NoContent();
        }
        
        /// <summary>
        /// Deactivate user
        /// </summary>
        [HttpPost("{userId}/deactivate")]
        [Authorize]
        public async Task<ActionResult> Deactivate(long userId)
        {
            var command = new DeactivateUserCommand { UserId = userId };
            await _mediator.Send(command);
            
            return NoContent();
        }
        
        /// <summary>
        /// Assign role to user
        /// </summary>
        [HttpPost("{userId}/assign-role")]
        [Authorize]
        public async Task<ActionResult> AssignRole(long userId, AssignRoleToUserCommand command)
        {
            if (userId != command.UserId)
            {
                return BadRequest("User ID in the URL does not match the User ID in the request body");
            }
            
            await _mediator.Send(command);
            
            return NoContent();
        }
        
        /// <summary>
        /// Remove role from user
        /// </summary>
        [HttpPost("{userId}/remove-role")]
        [Authorize]
        public async Task<ActionResult> RemoveRole(long userId, RevokeRoleFromUserCommand command)
        {
            if (userId != command.UserId)
            {
                return BadRequest("User ID in the URL does not match the User ID in the request body");
            }
            
            await _mediator.Send(command);
            
            return NoContent();
        }
    }
}