using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Features.Permission;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Domain.Events;
using BloggingSystem.Domain.Exceptions;
using BloggingSystem.Shared.DTOs;
using BloggingSystem.Application.Features.Role;
using BloggingSystem.Application.Features.RolePermission;
using MediatR;

namespace BloggingSystem.Application.Commands;

public class CreateRoleCommand: IRequest<RoleDto>
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }
}

public class UpdateRoleCommand: IRequest<RoleDto>
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleDto>
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IDomainEventService _domainEventService;
    
    public CreateRoleCommandHandler(IRepository<Role> roleRepository, IDomainEventService domainEventService)
    {
        _roleRepository = roleRepository;
        _domainEventService = domainEventService;
    }
    public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name)) {
            throw new ArgumentException("Permission name cannot be null or empty.", nameof(request.Name));
        }
        var roleByName = new RoleByNameSpecification(request.Name);
        
        if (await _roleRepository.AnyAsync(roleByName))
            throw new DomainException("Role with the same name already exists");

        var roleBySlug = new RoleBySlugSpecification(request.Slug);
        
        if (await _roleRepository.AnyAsync(roleBySlug))
            throw new DomainException("Role with the same slug already exists");
        
        var role = Role.Create(request.Name, request.Description);
        
        await _roleRepository.AddAsync(role, cancellationToken);
        await _domainEventService.PublishEventsAsync(role.DomainEvents);
        
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Slug = role.Slug,
            Description = role.Description
        };
    }
}

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, RoleDto>
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IDomainEventService _domainEventService;
    
    public UpdateRoleCommandHandler(IRepository<Role> roleRepository, IDomainEventService domainEventService)
    {
        _roleRepository = roleRepository;
        _domainEventService = domainEventService;
    }
    
    public async Task<RoleDto> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.Id);
        
        if (role == null)
            throw new DomainException("Role not found");

        role.Update(request.Name, request.Description);
        
        await _roleRepository.UpdateAsync(role);
        await _domainEventService.PublishEventsAsync(role.DomainEvents);
        
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Slug = role.Slug,
            Description = role.Description
        };
    }
}

public class GrantPermissionToRoleCommand: IRequest<bool>
{
    public long RoleId { get; set; }
    public long PermissionId { get; set; }
}

public class RevokePermissionFromRoleCommand: IRequest<bool>
{
    public long RoleId { get; set; }
    public long PermissionId { get; set; }
}

public class GrantPermissionToRoleCommandHandler : IRequestHandler<GrantPermissionToRoleCommand, bool>
{
    private readonly IRepository<RolePermission> _rolePermissionRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IDomainEventService _domainEventService;
    
    public GrantPermissionToRoleCommandHandler(
        IRepository<RolePermission> rolePermissionRepository,
        IRepository<Role> roleRepository,
        IRepository<Permission> permissionRepository,
        IDomainEventService domainEventService)
    {
        _rolePermissionRepository = rolePermissionRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _domainEventService = domainEventService;
    }
    
    public async Task<bool> Handle(GrantPermissionToRoleCommand request, CancellationToken cancellationToken)
    {
        var roleSpec = new RoleByIdSpecification(request.RoleId);
        var permissionSpec = new PermissionByIdSpecification(request.PermissionId);
        
        var role = await _roleRepository.FirstOrDefaultAsync(roleSpec, cancellationToken);
        var permission = await _permissionRepository.FirstOrDefaultAsync(permissionSpec, cancellationToken);
        
        if (role == null)
            throw new DomainException("Role not found");
        
        if (permission == null)
            throw new DomainException("Permission not found");
        
        // Check if the role already has the permission
        var rolePermissionSpec = new RolePermissionByRoleAndPermissionSpecification(role.Id, permission.Id);
        if (await _rolePermissionRepository.AnyAsync(rolePermissionSpec))
            throw new DomainException("Role already has the specified permission");
        
        var rolePermission = RolePermission.Create(request.RoleId, request.PermissionId);
        
        await _rolePermissionRepository.AddAsync(rolePermission, cancellationToken);

        await _domainEventService.PublishEventsAsync(rolePermission.DomainEvents);

        return true;
    }
}

public class RevokePermissionFromRoleCommandHandler : IRequestHandler<RevokePermissionFromRoleCommand, bool>
{
    private readonly IRepository<RolePermission> _rolePermissionRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IDomainEventService _domainEventService;
    
    public RevokePermissionFromRoleCommandHandler(
        IRepository<RolePermission> rolePermissionRepository,
        IRepository<Role> roleRepository,
        IRepository<Permission> permissionRepository,
        IDomainEventService domainEventService)
    {
        _rolePermissionRepository = rolePermissionRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _domainEventService = domainEventService;
    }
    
    public async Task<bool> Handle(RevokePermissionFromRoleCommand request, CancellationToken cancellationToken)
    {
        var roleSpec = new RoleByIdSpecification(request.RoleId);
        var permissionSpec = new PermissionByIdSpecification(request.PermissionId);
        
        var role = await _roleRepository.FirstOrDefaultAsync(roleSpec, cancellationToken);
        var permission = await _permissionRepository.FirstOrDefaultAsync(permissionSpec, cancellationToken);
        
        if (role == null)
            throw new DomainException("Role not found");
        
        if (permission == null)
            throw new DomainException("Permission not found");
        
        var rolePermission = await _rolePermissionRepository.FirstOrDefaultAsync(
            new RolePermissionByRoleAndPermissionSpecification(role.Id, permission.Id), cancellationToken);
        
        if (rolePermission == null)
            throw new DomainException("Role permission not found");

        rolePermission.Delete(roleId: request.RoleId, permissionId: request.PermissionId);
        
        await _rolePermissionRepository.DeleteAsync(rolePermission, cancellationToken);
        
        await _domainEventService.PublishEventsAsync(rolePermission.DomainEvents);

        return true;
    }
}