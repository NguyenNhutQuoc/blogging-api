using BloggingSystem.Application.Features.Media.Commands;
using BloggingSystem.Application.Features.Media.Queries;
using BloggingSystem.Shared.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloggingSystem.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/media")]
    [Authorize]
    public class MediaController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MediaController> _logger;

        public MediaController(IMediator mediator, ILogger<MediaController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Upload a file
        /// </summary>
        [HttpPost("upload")]
        [ProducesResponseType(typeof(MediaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MediaDto>> UploadFile([FromForm] UploadFileRequest file, [FromForm] string folder = "uploads")
        {
            if (file.File == null || file.File.Length == 0)
                return BadRequest("No file uploaded");

            // Check file size (max 10MB)
            if (file.File.Length > 10 * 1024 * 1024)
                return BadRequest("File size exceeds the limit (10MB)");

            // Check file type
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!Array.Exists(allowedTypes, type => type.Equals(file.File.ContentType, StringComparison.OrdinalIgnoreCase)))
                return BadRequest("File type not allowed. Supported types: JPEG, PNG, GIF, WEBP");

            try
            {
                var command = new UploadMediaCommand
                {
                    File = file.File,
                    Folder = folder
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error uploading file");
            }
        }

        /// <summary>
        /// Delete a media file
        /// </summary>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFile(long id)
        {
            try
            {
                var command = new DeleteMediaCommand { Id = id };
                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting media {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting media");
            }
        }

        /// <summary>
        /// Get media details by ID
        /// </summary>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(MediaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MediaDto>> GetMediaById(long id)
        {
            var query = new GetMediaByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get all media with pagination
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponseDto<MediaDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponseDto<MediaDto>>> GetMedia(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetMediaQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get media files for the current user
        /// </summary>
        [HttpGet("my-media")]
        [ProducesResponseType(typeof(PaginatedResponseDto<MediaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedResponseDto<MediaDto>>> GetMyMedia(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetCurrentUserMediaQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}