using AutoMapper;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Shared.DTOs;
using BloggingSystem.Shared.Exceptions;
using MediatR;

namespace BloggingSystem.Application.Commands;

#region Create a mew Permission

public class CreatePermissionCommand : IRequest<PermissionDto>
{
    public string? Name { get; set; }
    public string? Module { get; set; }
    public string? Description { get; set; }
}

public class CreatePermissionCommandHandler : IRequestHandler<CreatePermissionCommand, PermissionDto>
{
    private readonly IMapper _mapper;
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IDomainEventService _domainEventService;
    
    public CreatePermissionCommandHandler(IRepository<Permission> permissionRepository, IDomainEventService domainEventService, IMapper mapper)
    {
        _permissionRepository = permissionRepository;
        _domainEventService = domainEventService;
        _mapper = mapper;
    }
    
    public async Task<PermissionDto> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Permission name cannot be null or empty.", nameof(request.Name));
        
        if (string.IsNullOrWhiteSpace(request.Module))
            throw new ArgumentException("Permission module cannot be null or empty.", nameof(request.Module));
        
        var permission = Permission.Create(request.Name, request.Module, request.Description);
        
        await _permissionRepository.AddAsync(permission, cancellationToken);

        await _domainEventService.PublishEventsAsync(permission.DomainEvents);
        
        return _mapper.Map<PermissionDto>(permission);
    }
}

#endregion

#region Update a Permission

public class UpdatePermissionCommand : IRequest<PermissionDto>
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Module { get; set; }
    public string? Description { get; set; }
}

public class UpdatePermissionCommandHandler : IRequestHandler<UpdatePermissionCommand, PermissionDto>
{
    private readonly IMapper _mapper;
    private readonly IDomainEventService _domainEventService;
    private readonly IRepository<Permission> _permissionRepository;
    
    public UpdatePermissionCommandHandler(IRepository<Permission> permissionRepository, IDomainEventService domainEventService, IMapper mapper)
    {
        _permissionRepository = permissionRepository;
        _mapper = mapper;
        _domainEventService = domainEventService;
    }
    
    public async Task<PermissionDto> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (permission == null)
            throw new NotFoundException(nameof(Permission), request.Id);
        
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Permission name cannot be null or empty.", nameof(request.Name));
        
        if (string.IsNullOrWhiteSpace(request.Module))
            throw new ArgumentException("Permission module cannot be null or empty.", nameof(request.Module));
        
        permission.Update(request.Name, request.Module, request.Description);
        
        await _permissionRepository.UpdateAsync(permission);

        await _domainEventService.PublishEventsAsync(permission.DomainEvents);
        
        return _mapper.Map<PermissionDto>(permission);
    }
}

#endregion

#region Delete a Permission

public class DeletePermissionCommand : IRequest<Unit>
{
    public long Id { get; set; }
}

public class DeletePermissionCommandHandler : IRequestHandler<DeletePermissionCommand, Unit>
{
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IDomainEventService _domainEventService;
    
    public DeletePermissionCommandHandler(IRepository<Permission> permissionRepository, IDomainEventService domainEventService)
    {
        _permissionRepository = permissionRepository;
        _domainEventService = domainEventService;
    }
    
    public async Task<Unit> Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (permission == null)
            throw new NotFoundException(nameof(Permission), request.Id);
        
        permission.Delete();
        
        await _permissionRepository.DeleteAsync(permission);

        await _domainEventService.PublishEventsAsync(permission.DomainEvents);
        
        return Unit.Value;
    }
}

#endregion