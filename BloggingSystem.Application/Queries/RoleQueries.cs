using AutoMapper;
using BloggingSystem.Application;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Features.Role;
using BloggingSystem.Application.Features.User;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Shared.Constants;
using BloggingSystem.Shared.DTOs;
using MediatR;

namespace BloggingSystem.Application.Queries;

public class GetRoleByIdQuery : IRequest<RoleDto>
{
    public long Id { get; }

    public GetRoleByIdQuery(long id)
    {
        Id = id;
    }
}

public class GetRolesQuery : IRequest<PaginatedResponseDto<RoleSummaryDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = AppConstants.DefaultPageSize;
    
}

public class GetRolesCountQuery : IRequest<int>
{
}

public class GetRolesByUserIdQuery : IRequest<PaginatedResponseDto<RoleDto>>
{
    public long UserId { get; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = AppConstants.DefaultPageSize;

    public GetRolesByUserIdQuery(long userId)
    {
        UserId = userId;
    }
}

public class GetRolesByUserEmailQuery : IRequest<PaginatedResponseDto<RoleDto>>
{
    public string Email { get; }

    public GetRolesByUserEmailQuery(string email)
    {
        Email = email;
    }
}

public class GetRolesByUserUsernameQuery : IRequest<PaginatedResponseDto<RoleDto>>
{
    public string Username { get; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = AppConstants.DefaultPageSize;

    public GetRolesByUserUsernameQuery(string username)
    {
        Username = username;
    }
}

public class SearchRolesQuery : IRequest<PaginatedResponseDto<RoleSummaryDto>>
{
    public string SearchTerm { get; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = AppConstants.DefaultPageSize;

    public SearchRolesQuery(string searchTerm)
    {
        SearchTerm = searchTerm;
    }
}

public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDto>
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IMapper _mapper;

    public GetRoleByIdQueryHandler(IRepository<Role> roleRepository, IMapper mapper)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    public async Task<RoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var roleSpec = new RoleByIdSpecification(request.Id);
        var role = await _roleRepository.FirstOrDefaultAsync(roleSpec, cancellationToken);
        return _mapper.Map<RoleDto>(role);
    }
}

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, PaginatedResponseDto<RoleSummaryDto>>
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IMapper _mapper;

    public GetRolesQueryHandler(IRepository<Role> roleRepository, IMapper mapper)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResponseDto<RoleSummaryDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var countSpec = new RoleSpecification();
        var totalItems = await _roleRepository.CountAsync(countSpec, cancellationToken);
        
        var rolesSpec = new RoleSpecification(request.PageNumber, request.PageSize);
        
        var roles = await _roleRepository.ListAsync(rolesSpec, cancellationToken);
        
        var roleDtos = _mapper.Map<List<RoleSummaryDto>>(roles);
        
        return new PaginatedResponseDto<RoleSummaryDto>
        {
            Data = roleDtos,
            TotalPages = totalItems,
            PageIndex = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

public class GetRolesCountQueryHandler : IRequestHandler<GetRolesCountQuery, int>
{
    private readonly IRepository<Role> _roleRepository;

    public GetRolesCountQueryHandler(IRepository<Role> roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<int> Handle(GetRolesCountQuery request, CancellationToken cancellationToken)
    {
        var countSpec = new RoleSpecification();
        return await _roleRepository.CountAsync(countSpec, cancellationToken);
    }
}

public class GetRolesByUserIdQueryHandler : IRequestHandler<GetRolesByUserIdQuery, PaginatedResponseDto<RoleDto>>
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IMapper _mapper;

    public GetRolesByUserIdQueryHandler(IRepository<Role> roleRepository, IMapper mapper)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResponseDto<RoleDto>> Handle(GetRolesByUserIdQuery request, CancellationToken cancellationToken)
    {
        var countSpec = new RolesWithUserSpecification(request.UserId);
        var totalItems = await _roleRepository.CountAsync(countSpec, cancellationToken);
        
        var rolesSpec = new RolesWithUserSpecification(request.UserId, request.PageNumber, request.PageSize);
        
        var roles = await _roleRepository.ListAsync(rolesSpec, cancellationToken);
        
        var roleDtos = _mapper.Map<List<RoleDto>>(roles);
        
        return new PaginatedResponseDto<RoleDto>
        {
            Data = roleDtos,
            TotalPages = totalItems,
            PageIndex = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

public class GetRolesByUserEmailQueryHandler : IRequestHandler<GetRolesByUserEmailQuery, PaginatedResponseDto<RoleDto>>
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IMapper _mapper;

    public GetRolesByUserEmailQueryHandler(IRepository<Role> roleRepository, IRepository<User> userRepository, IMapper mapper)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResponseDto<RoleDto>> Handle(GetRolesByUserEmailQuery request, CancellationToken cancellationToken)
    {
        var userSpec = new UserByEmailSpecification(request.Email);
        var user = await _userRepository.FirstOrDefaultAsync(userSpec, cancellationToken);
        
        if (user == null)
        {
            return new PaginatedResponseDto<RoleDto>
            {
                Data = new List<RoleDto>(),
                TotalPages = 0,
                PageIndex = 1,
                PageSize = AppConstants.DefaultPageSize
            };
        }
        
        var countSpec = new RolesWithUserSpecification(user.Id);
        var totalItems = await _roleRepository.CountAsync(countSpec, cancellationToken);
        
        var rolesSpec = new RolesWithUserSpecification(user.Id);
        
        var roles = await _roleRepository.ListAsync(rolesSpec, cancellationToken);
        
        var roleDtos = _mapper.Map<List<RoleDto>>(roles);
        
        return new PaginatedResponseDto<RoleDto>
        {
            Data = roleDtos,
            TotalPages = totalItems,
            PageIndex = 1,
            PageSize = AppConstants.DefaultPageSize
        };
    }
}

public class GetRolesByUserUsernameQueryHandler : IRequestHandler<GetRolesByUserUsernameQuery, PaginatedResponseDto<RoleDto>>
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IMapper _mapper;

    public GetRolesByUserUsernameQueryHandler(IRepository<Role> roleRepository, IRepository<User> userRepository, IMapper mapper)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResponseDto<RoleDto>> Handle(GetRolesByUserUsernameQuery request, CancellationToken cancellationToken)
    {
        var userSpec = new UserByUsernameSpecification(request.Username);
        var user = await _userRepository.FirstOrDefaultAsync(userSpec, cancellationToken);
        
        if (user == null)
        {
            return new PaginatedResponseDto<RoleDto>
            {
                Data = new List<RoleDto>(),
                TotalPages = 0,
                PageIndex = 1,
                PageSize = AppConstants.DefaultPageSize
            };
        }
        
        var countSpec = new RolesWithUserSpecification(user.Id);
        var totalItems = await _roleRepository.CountAsync(countSpec, cancellationToken);
        
        var rolesSpec = new RolesWithUserSpecification(user.Id);
        
        var roles = await _roleRepository.ListAsync(rolesSpec, cancellationToken);
        
        var roleDtos = _mapper.Map<List<RoleDto>>(roles);
        
        return new PaginatedResponseDto<RoleDto>
        {
            Data = roleDtos,
            TotalPages = totalItems,
            PageIndex = 1,
            PageSize = AppConstants.DefaultPageSize
        };
    }
}

public class SearchRolesQueryHandler : IRequestHandler<SearchRolesQuery, PaginatedResponseDto<RoleSummaryDto>>
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IMapper _mapper;

    public SearchRolesQueryHandler(IRepository<Role> roleRepository, IMapper mapper)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResponseDto<RoleSummaryDto>> Handle(SearchRolesQuery request, CancellationToken cancellationToken)
    {
        var countSpec = new RoleSearchSpecification(request.SearchTerm);
        var totalItems = await _roleRepository.CountAsync(countSpec, cancellationToken);
        
        var rolesSpec = new RoleSearchSpecification(request.SearchTerm, request.PageNumber, request.PageSize);
        
        var roles = await _roleRepository.ListAsync(rolesSpec, cancellationToken);
        
        var roleDtos = _mapper.Map<List<RoleSummaryDto>>(roles);
        
        return new PaginatedResponseDto<RoleSummaryDto>
        {
            Data = roleDtos,
            TotalPages = totalItems,
            PageIndex = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

