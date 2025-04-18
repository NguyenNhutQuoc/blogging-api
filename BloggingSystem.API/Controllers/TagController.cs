using System.Threading.Tasks;
using BloggingSystem.Application.Features.Tag.Queries;
using BloggingSystem.Shared.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/tags")]
    public class TagController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TagController> _logger;

        public TagController(IMediator mediator, ILogger<TagController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get all tags without pagination
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(typeof(List<TagDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TagDto>>> GetAllTags()
        {
            var query = new GetAllTagsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get all tags with pagination
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponseDto<TagDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponseDto<TagDto>>> GetTags(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetTagsQuery()
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get a tag by ID
        /// </summary>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(TagDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TagDto>> GetTagById(long id)
        {
            var query = new GetTagByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 