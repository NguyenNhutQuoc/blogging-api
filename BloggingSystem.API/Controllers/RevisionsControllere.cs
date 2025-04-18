using System.Threading.Tasks;
using BloggingSystem.Application.Commands;
using BloggingSystem.Application.Features.Revisions.Command;
using BloggingSystem.Application.Features.Revisions.Queries;
using BloggingSystem.Application.Queries;
using BloggingSystem.Shared.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/revisions")]
    public class RevisionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RevisionController> _logger;

        public RevisionController(IMediator mediator, ILogger<RevisionController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get revisions for a specific post
        /// </summary>
        [HttpGet("post/{postId}")]
        [ProducesResponseType(typeof(PaginatedResponseDto<RevisionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaginatedResponseDto<RevisionDto>>> GetRevisionsByPost(
            long postId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetRevisionsByPostQuery
            {
                PostId = postId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get a specific revision by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RevisionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RevisionDto>> GetRevisionById(long id)
        {
            var query = new GetRevisionByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Compare two revisions
        /// </summary>
        [HttpGet("compare")]
        [ProducesResponseType(typeof(RevisionComparisonDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RevisionComparisonDto>> CompareRevisions(
            [FromQuery] long sourceRevisionId,
            [FromQuery] long targetRevisionId)
        {
            var query = new CompareRevisionsQuery
            {
                SourceRevisionId = sourceRevisionId,
                TargetRevisionId = targetRevisionId
            };
            
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Restore a post to a specific revision
        /// </summary>
        [HttpPost("{id}/restore")]
        [Authorize(Policy = "Permission:post.restore-revision")]
        [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostDto>> RestoreRevision(long id)
        {
            var command = new RestoreRevisionCommand() { RevisionId = id };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Get latest revision for a post
        /// </summary>
        [HttpGet("post/{postId}/latest")]
        [Authorize(Policy = "Permission:post.view-revisions")]
        [ProducesResponseType(typeof(RevisionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RevisionDto>> GetLatestRevision(long postId)
        {
            var query = new GetLatestRevisionByPostQuery { PostId = postId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get revision count for a post
        /// </summary>
        [HttpGet("post/{postId}/count")]
        [Authorize(Policy = "Permission:post.view-revisions")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetRevisionCount(long postId)
        {
            var query = new GetRevisionCountByPostQuery() { PostId = postId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Compore revision with post
        /// </summary>
        [HttpGet("post/{postId}/compare")]
        public async Task<ActionResult<RevisionComparisonDto>> CompareRevisionWithPost(
            long postId,
            [FromQuery] long revisionId)
        {
            var query = new CompareRevisionsByPostQuery
            {
                PostId = postId,
                SourceRevisionId = revisionId
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

    }
}