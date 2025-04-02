using MediatR;
using Microsoft.AspNetCore.Mvc;
using BloggingSystem.Application.Commands;
using BloggingSystem.Application.Queries;
using BloggingSystem.Shared.DTOs;

namespace BloggingSystem.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/roles")]
public class RoleController: ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;
    
    public RoleController(IMediator mediator, ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<ActionResult<PaginatedResponseDto<RoleSummaryDto>>> GetRoles([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetRolesQuery 
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<RoleDto>> GetRoleById(long id)
    {
        var query = new GetRoleByIdQuery(id);
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }
    
    [HttpGet("count")]
    public async Task<ActionResult<int>> GetRolesCount()
    {
        var query = new GetRolesCountQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("search")]
    public async Task<ActionResult<PaginatedResponseDto<RoleSummaryDto>>> SearchRoles([FromQuery] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new SearchRolesQuery(searchTerm) { PageNumber = pageNumber, PageSize = pageSize };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<PaginatedResponseDto<RoleDto>>> GetRolesByUserId(long userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetRolesByUserIdQuery(userId) { PageNumber = pageNumber, PageSize = pageSize };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("user/email/{email}")]
    public async Task<ActionResult<PaginatedResponseDto<RoleDto>>> GetRolesByUserEmail(string email)
    {
        var query = new GetRolesByUserEmailQuery(email);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("user/username/{username}")]
    public async Task<ActionResult<PaginatedResponseDto<RoleDto>>> GetRolesByUserUsername(string username, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetRolesByUserUsernameQuery(username) { PageNumber = pageNumber, PageSize = pageSize };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetRoleById), new { id = result.Id }, result);
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<RoleDto>> UpdateRole(long id, [FromBody] UpdateRoleCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID in the URL does not match the ID in the request body");
            
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    [HttpPost("{id}/grant-permission")]
    public async Task<ActionResult<RoleDto>> GrantPermissionToRole(long id, [FromBody] GrantPermissionToRoleCommand command)
    {
        if (id != command.RoleId)
            return BadRequest("ID in the URL does not match the ID in the request body");
        
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    [HttpPost("{id}/revoke-permission")]
    public async Task<ActionResult<RoleDto>> RevokePermissionFromRole(long id, [FromBody] RevokePermissionFromRoleCommand command)
    {
        if (id != command.RoleId)
            return BadRequest("ID in the URL does not match the ID in the request body");
        
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}