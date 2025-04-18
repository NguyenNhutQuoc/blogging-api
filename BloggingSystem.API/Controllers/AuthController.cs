using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using BloggingSystem.Application.Authentication.Commands;
using BloggingSystem.Shared.Exceptions;
using BloggingSystem.Domain.Exceptions;

namespace BloggingSystem.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.Password != request.ConfirmPassword)
                return BadRequest(new { message = "Passwords do not match" });

            try
            {
                var command = new RegisterCommand
                {
                    Username = request.Username,
                    Email = request.Email,
                    Password = request.Password,
                    DisplayName = request.Username,
                    Bio = request.Bio,
                    Image = request.Image,
                    IpAddress = GetIpAddress(),
                    UserAgent = GetUserAgent()
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (DomainException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred during registration" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var command = new LoginCommand
                {
                    Email = request.Email,
                    Password = request.Password,
                    IpAddress = GetIpAddress(),
                    UserAgent = GetUserAgent()
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var command = new RefreshTokenCommand
                {
                    RefreshToken = request.RefreshToken,
                    IpAddress = GetIpAddress(),
                    UserAgent = GetUserAgent()
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred during token refresh" });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !long.TryParse(userId, out var userIdLong))
                    return Unauthorized();

                var command = new LogoutCommand
                {
                    UserId = userIdLong,
                    RefreshToken = request.RefreshToken
                };

                var success = await _mediator.Send(command);
                if (!success)
                    return BadRequest(new { message = "Invalid token" });

                return Ok(new { message = "Logout successful" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred during logout" });
            }
        }

        [HttpPost("social/google")]
        public async Task<IActionResult> GoogleLogin([FromBody] SocialLoginRequest request)
        {
            if (request.Provider != "google" || string.IsNullOrEmpty(request.AccessToken))
                return BadRequest(new { message = "Invalid request" });

            try
            {
                var command = new GoogleAuthCommand
                {
                    AccessToken = request.AccessToken,
                    IpAddress = GetIpAddress(),
                    UserAgent = GetUserAgent()
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred during Google authentication" });
            }
        }

        private string GetIpAddress()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        private string GetUserAgent()
        {
            return HttpContext.Request.Headers["User-Agent"].ToString();
        }
    }

    // Request models
    public class RegisterUserRequest
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
        public string? Bio { get; set; }
        public string? Image { get; set; }
        public string? DisplayName { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = null!;
    }

    public class LogoutRequest
    {
        public string RefreshToken { get; set; } = null!;
    }

    public class SocialLoginRequest
    {
        public string Provider { get; set; } = null!;
        public string AccessToken { get; set; } = null!;
        public string? Code { get; set; }
    }
}