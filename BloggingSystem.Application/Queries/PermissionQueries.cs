using AutoMapper;
using BloggingSystem.Application;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Features.Permission;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Shared.DTOs;
using BloggingSystem.Shared.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Application.Queries;

#region  Get Permissions

public class GetPermissionsQuery : IRequest<PaginatedResponseDto<PermissionDto>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, PaginatedResponseDto<PermissionDto>>
{
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetPermissionsQueryHandler> _logger;
    
    public GetPermissionsQueryHandler(IRepository<Permission> permissionRepository, IMapper mapper, ILogger<GetPermissionsQueryHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<PaginatedResponseDto<PermissionDto>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        var spec = new PermissionsSpecification(request.PageNumber, request.PageSize);
        var totalCount = await _permissionRepository.CountAsync(spec, cancellationToken);
        var permissions = await _permissionRepository.ListAsync(spec, cancellationToken);
        
        return new PaginatedResponseDto<PermissionDto>
        {
            Data = _mapper.Map<List<PermissionDto>>(permissions),
            PageIndex = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}

#endregion

#region Get Permission by Id

public class GetPermissionByIdQuery : IRequest<PermissionDto>
{
    public long Id { get; set; }
}

public class GetPermissionByIdQueryHandler : IRequestHandler<GetPermissionByIdQuery, PermissionDto>
{
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IMapper _mapper;
    
    public GetPermissionByIdQueryHandler(IRepository<Permission> permissionRepository, IMapper mapper, ILogger<GetUserByIdQueryHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _mapper = mapper;
    }
    
    public async Task<PermissionDto> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new PermissionByIdSpecification(request.Id);

        var permission = await _permissionRepository.FirstOrDefaultAsync(spec, cancellationToken);
        
        if (permission == null)
        {
            throw new NotFoundException(nameof(Permission), request.Id);
        }
        
        return _mapper.Map<PermissionDto>(permission);
    }
}

#endregion

#region Get Permission by Slug

public class GetPermissionBySlugQuery : IRequest<PermissionDto>
{
    public string Slug { get; set; }
}

public class GetPermissionBySlugQueryHandler : IRequestHandler<GetPermissionBySlugQuery, PermissionDto>
{
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IMapper _mapper;
    
    public GetPermissionBySlugQueryHandler(IRepository<Permission> permissionRepository, IMapper mapper, ILogger<GetUserByIdQueryHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _mapper = mapper;
    }
    
    public async Task<PermissionDto> Handle(GetPermissionBySlugQuery request, CancellationToken cancellationToken)
    {
        var spec = new PermissionBySlugSpecification(request.Slug);

        var permission = await _permissionRepository.FirstOrDefaultAsync(spec, cancellationToken);
        
        if (permission == null)
        {
            throw new NotFoundException(nameof(Permission), request.Slug);
        }
        
        
        return _mapper.Map<PermissionDto>(permission);
    }
}

#endregion

#region Get Permission by Name

public class GetPermissionByNameQuery : IRequest<PermissionDto>
{
    public string Name { get; set; }
}

public class GetPermissionByNameQueryHandler : IRequestHandler<GetPermissionByNameQuery, PermissionDto>
{
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IMapper _mapper;
    
    public GetPermissionByNameQueryHandler(IRepository<Permission> permissionRepository, IMapper mapper, ILogger<GetUserByIdQueryHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _mapper = mapper;
    }
    
    public async Task<PermissionDto> Handle(GetPermissionByNameQuery request, CancellationToken cancellationToken)
    {
        var spec = new PermissionByNameSpecification(request.Name);

        var permission = await _permissionRepository.FirstOrDefaultAsync(spec, cancellationToken);
        
        if (permission == null)
        {
            throw new NotFoundException(nameof(Permission), request.Name);
        }
        
        return _mapper.Map<PermissionDto>(permission);
    }
}

#endregion

#region Get Permissions by Role

public class GetPermissionsByRoleQuery : IRequest<PaginatedResponseDto<PermissionDto>>
{
    public long RoleId { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class GetPermissionsByRoleQueryHandler : IRequestHandler<GetPermissionsByRoleQuery, PaginatedResponseDto<PermissionDto>>
{
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IMapper _mapper;
    
    public GetPermissionsByRoleQueryHandler(IRepository<Permission> permissionRepository, IMapper mapper, ILogger<GetUserByIdQueryHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _mapper = mapper;
    }
    
    public async Task<PaginatedResponseDto<PermissionDto>> Handle(GetPermissionsByRoleQuery request, CancellationToken cancellationToken)
    {
        var specCount = new PermissionsWithRoleSpecification(request.RoleId);
        var totalCount = await _permissionRepository.CountAsync(specCount, cancellationToken);
        var spec = new PermissionsWithRoleSpecification(request.RoleId, request.PageNumber, request.PageSize);

        var permissions = await _permissionRepository.ListAsync(spec, cancellationToken);
        
        return new PaginatedResponseDto<PermissionDto>
        {
            Data = _mapper.Map<List<PermissionDto>>(permissions),
            PageIndex = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}

#endregion

#region  Get Permissions By User Id
public class GetPermissionsByUserIdQuery : IRequest<PaginatedResponseDto<PermissionDto>>
{
    public long UserId { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class GetPermissionsByUserIdQueryHandler : IRequestHandler<GetPermissionsByUserIdQuery, PaginatedResponseDto<PermissionDto>>
{
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IMapper _mapper;
    
    public GetPermissionsByUserIdQueryHandler(IRepository<Permission> permissionRepository, IMapper mapper, ILogger<GetUserByIdQueryHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _mapper = mapper;
    }
    
    public async Task<PaginatedResponseDto<PermissionDto>> Handle(GetPermissionsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new UserPermissionSpecification(request.UserId);
        var totalCount = await _permissionRepository.CountAsync(spec, cancellationToken);
        var specPaged = new UserPermissionSpecification(request.UserId, request.PageNumber, request.PageSize);

        var permissions = await _permissionRepository.ListAsync(specPaged, cancellationToken);
        
        return new PaginatedResponseDto<PermissionDto>
        {
            Data = _mapper.Map<List<PermissionDto>>(permissions),
            PageIndex = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}

#endregion

#region Get Permissions by Module

public class GetPermissionsByModuleQuery : IRequest<PaginatedResponseDto<PermissionDto>>
{
    public string Module { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class GetPermissionsByModuleQueryHandler : IRequestHandler<GetPermissionsByModuleQuery, PaginatedResponseDto<PermissionDto>>
{
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IMapper _mapper;
    
    public GetPermissionsByModuleQueryHandler(IRepository<Permission> permissionRepository, IMapper mapper, ILogger<GetUserByIdQueryHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _mapper = mapper;
    }
    
    public async Task<PaginatedResponseDto<PermissionDto>> Handle(GetPermissionsByModuleQuery request, CancellationToken cancellationToken)
    {
        var spec = new PermissionByModuleSpecification(request.Module);
        var totalCount = await _permissionRepository.CountAsync(spec, cancellationToken);
        var specPaged = new PermissionByModuleSpecification(request.Module, request.PageNumber, request.PageSize);

        var permissions = await _permissionRepository.ListAsync(specPaged, cancellationToken);
        
        return new PaginatedResponseDto<PermissionDto>
        {
            Data = _mapper.Map<List<PermissionDto>>(permissions),
            PageIndex = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}

#endregion

#region  Search Permissions
public class SearchPermissionsQuery : IRequest<PaginatedResponseDto<PermissionDto>>
{
    public string SearchTerm { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class SearchPermissionsQueryHandler : IRequestHandler<SearchPermissionsQuery, PaginatedResponseDto<PermissionDto>>
{
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IMapper _mapper;
    
    public SearchPermissionsQueryHandler(IRepository<Permission> permissionRepository, IMapper mapper, ILogger<GetUserByIdQueryHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _mapper = mapper;
    }
    
    public async Task<PaginatedResponseDto<PermissionDto>> Handle(SearchPermissionsQuery request, CancellationToken cancellationToken)
    {
        var spec = new PermissionSearchSpecification(request.SearchTerm);
        var totalCount = await _permissionRepository.CountAsync(spec, cancellationToken);
        var specPaged = new PermissionSearchSpecification(request.SearchTerm, request.PageNumber, request.PageSize);

        var permissions = await _permissionRepository.ListAsync(specPaged, cancellationToken);
        
        return new PaginatedResponseDto<PermissionDto>
        {
            Data = _mapper.Map<List<PermissionDto>>(permissions),
            PageIndex = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}

#endregion

#region Get Permissions Count

public class GetPermissionsCountQuery : IRequest<int>
{
}

public class GetPermissionsCountQueryHandler : IRequestHandler<GetPermissionsCountQuery, int>
{
    private readonly IRepository<Permission> _permissionRepository;
    public GetPermissionsCountQueryHandler(IRepository<Permission> permissionRepository, ILogger<GetUserByIdQueryHandler> logger)
    {
        _permissionRepository = permissionRepository;
    }
    public async Task<int> Handle(GetPermissionsCountQuery request, CancellationToken cancellationToken)
    {
        var spec = new PermissionsSpecification();
        return await _permissionRepository.CountAsync(spec, cancellationToken);
    }
}

#endregion
