using BloggingSystem.Application.Commands;
using BloggingSystem.Application.Queries;
using BloggingSystem.Shared.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BloggingSystem.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/permissions")]
public class PermissionController: ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PermissionController> _logger;
    
    public PermissionController(IMediator mediator, ILogger<PermissionController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<ActionResult<PaginatedResponseDto<PermissionDto>>> GetPermissions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetPermissionsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<PermissionDto>> GetPermissionById(long id)
    {
        var query = new GetPermissionByIdQuery
        {
            Id = id
        };
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }
    
    [HttpGet("count")]
    public async Task<ActionResult<int>> GetPermissionsCount()
    {
        var query = new GetPermissionsCountQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("search")]
    public async Task<ActionResult<PaginatedResponseDto<PermissionDto>>> SearchPermissions([FromQuery] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new SearchPermissionsQuery
        {
            SearchTerm = searchTerm,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("role/{roleId}")]
    public async Task<ActionResult<PaginatedResponseDto<PermissionDto>>> GetPermissionsByRoleId(long roleId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetPermissionsByRoleQuery
        {
            RoleId = roleId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<ActionResult<PermissionDto>> CreatePermission([FromBody] CreatePermissionCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<PermissionDto>> UpdatePermission(long id, [FromBody] UpdatePermissionCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}