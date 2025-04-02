using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BloggingSystem.Application;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Features.User;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Domain.Exceptions;
using BloggingSystem.Shared.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Application.Queries
{
    #region Get User By ID
    
    public class GetUserByIdQuery : IRequest<UserDto>
    {
        public long UserId { get; set; }
    }
    
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserByIdQueryHandler> _logger;
        
        public GetUserByIdQueryHandler(
            IRepository<User> userRepository,
            IMapper mapper,
            ILogger<GetUserByIdQueryHandler> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            
            if (user == null)
            {
                throw new EntityNotFoundException(nameof(User), request.UserId);
            }
            
            return _mapper.Map<UserDto>(user);
        }
    }
    
    #endregion
    
    #region Get User By Username
    
    public class GetUserByUsernameQuery : IRequest<UserDto>
    {
        public string Username { get; set; }
    }
    
    public class GetUserByUsernameQueryHandler : IRequestHandler<GetUserByUsernameQuery, UserDto>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserByUsernameQueryHandler> _logger;
        
        public GetUserByUsernameQueryHandler(
            IRepository<User> userRepository,
            IMapper mapper,
            ILogger<GetUserByUsernameQueryHandler> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<UserDto> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
        {
            var spec = new UserByUsernameSpecification(request.Username);
            var user = await _userRepository.FirstOrDefaultAsync(spec, cancellationToken);
            
            if (user == null)
            {
                throw new EntityNotFoundException(nameof(User), request.Username);
            }
            
            return _mapper.Map<UserDto>(user);
        }
    }
    
    #endregion
    
    #region Get User By Email
    
    public class GetUserByEmailQuery : IRequest<UserDto>
    {
        public string Email { get; set; }
    }
    
    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, UserDto>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserByEmailQueryHandler> _logger;
        
        public GetUserByEmailQueryHandler(
            IRepository<User> userRepository,
            IMapper mapper,
            ILogger<GetUserByEmailQueryHandler> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<UserDto> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            var spec = new UserByEmailSpecification(request.Email);
            var user = await _userRepository.FirstOrDefaultAsync(spec, cancellationToken);
            
            if (user == null)
            {
                throw new EntityNotFoundException(nameof(User), request.Email);
            }
            
            return _mapper.Map<UserDto>(user);
        }
    }
    
    #endregion
    
    #region Get Active Users
    
    public class GetActiveUsersQuery : IRequest<PaginatedResponseDto<UserSummaryDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    
    public class GetActiveUsersQueryHandler : IRequestHandler<GetActiveUsersQuery, PaginatedResponseDto<UserSummaryDto>>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetActiveUsersQueryHandler> _logger;
        
        public GetActiveUsersQueryHandler(
            IRepository<User> userRepository,
            IMapper mapper,
            ILogger<GetActiveUsersQueryHandler> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<PaginatedResponseDto<UserSummaryDto>> Handle(GetActiveUsersQuery request, CancellationToken cancellationToken)
        {
            // Get total count
            var countSpec = new ActiveUsersSpecification();
            var totalCount = await _userRepository.CountAsync(countSpec, cancellationToken);
            
            // Get paged items
            var pagedSpec = new ActiveUsersSpecification(request.PageNumber, request.PageSize);
            var users = await _userRepository.ListAsync(pagedSpec, cancellationToken);
            
            // Map to DTOs
            var userDtos = _mapper.Map<List<UserSummaryDto>>(users);
            
            // Create paginated response
            return new PaginatedResponseDto<UserSummaryDto>
            {
                Data = userDtos,
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }
    
    #endregion
    
    #region Get Users By Role
    
    public class GetUsersByRoleQuery : IRequest<PaginatedResponseDto<UserSummaryDto>>
    {
        public long RoleId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    
    public class GetUsersByRoleQueryHandler : IRequestHandler<GetUsersByRoleQuery, PaginatedResponseDto<UserSummaryDto>>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUsersByRoleQueryHandler> _logger;
        
        public GetUsersByRoleQueryHandler(
            IRepository<User> userRepository,
            IMapper mapper,
            ILogger<GetUsersByRoleQueryHandler> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<PaginatedResponseDto<UserSummaryDto>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
        {
            // Get total count
            var countSpec = new UsersWithRoleSpecification(request.RoleId);
            var totalCount = await _userRepository.CountAsync(countSpec, cancellationToken);
            
            // Get paged items
            var pagedSpec = new UsersWithRoleSpecification(request.RoleId, request.PageNumber, request.PageSize);
            var users = await _userRepository.ListAsync(pagedSpec, cancellationToken);
            
            // Map to DTOs
            var userDtos = _mapper.Map<List<UserSummaryDto>>(users);
            
            // Create paginated response
            return new PaginatedResponseDto<UserSummaryDto>
            {
                Data = userDtos,
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }
    
    #endregion
    
    #region Search Users
    
    public class SearchUsersQuery : IRequest<PaginatedResponseDto<UserSummaryDto>>
    {
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    
    public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, PaginatedResponseDto<UserSummaryDto>>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SearchUsersQueryHandler> _logger;
        
        public SearchUsersQueryHandler(
            IRepository<User> userRepository,
            IMapper mapper,
            ILogger<SearchUsersQueryHandler> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<PaginatedResponseDto<UserSummaryDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
        {
            // Get total count
            var countSpec = new SearchUsersSpecification(request.SearchTerm);
            var totalCount = await _userRepository.CountAsync(countSpec, cancellationToken);
            
            // Get paged items
            var pagedSpec = new SearchUsersSpecification(request.SearchTerm, request.PageNumber, request.PageSize);
            var users = await _userRepository.ListAsync(pagedSpec, cancellationToken);
            
            // Map to DTOs
            var userDtos = _mapper.Map<List<UserSummaryDto>>(users);
            
            // Create paginated response
            return new PaginatedResponseDto<UserSummaryDto>
            {
                Data = userDtos,
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }
    
    #endregion
    
    #region Get User Profile
    
    public class GetUserProfileQuery : IRequest<UserProfileDto>
    {
        public long UserId { get; set; }
    }
    
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
    {
        private readonly IRepository<UserProfile> _profileRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserProfileQueryHandler> _logger;
        
        public GetUserProfileQueryHandler(
            IRepository<UserProfile> profileRepository,
            IMapper mapper,
            ILogger<GetUserProfileQueryHandler> logger)
        {
            _profileRepository = profileRepository;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var spec = new ProfileByUserIdSpecification(request.UserId);
            var profile = await _profileRepository.FirstOrDefaultAsync(spec, cancellationToken);
            
            if (profile == null)
            {
                throw new EntityNotFoundException(nameof(UserProfile), request.UserId);
            }
            
            return _mapper.Map<UserProfileDto>(profile);
        }
    }
    
    #endregion
    
    #region Get User Roles
    
    public class GetUserRolesQuery : IRequest<List<RoleDto>>
    {
        public long UserId { get; set; }
    }
    
    public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, List<RoleDto>>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserRolesQueryHandler> _logger;
        
        public GetUserRolesQueryHandler(
            IRepository<User> userRepository,
            IRepository<UserRole> userRoleRepository,
            IRepository<Role> roleRepository,
            IMapper mapper,
            ILogger<GetUserRolesQueryHandler> logger)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<List<RoleDto>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            // Check if user exists
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new EntityNotFoundException(nameof(User), request.UserId);
            }
            
            // Get user roles
            var userRolesSpec = new UserRolesSpecification(request.UserId);
            var userRoles = await _userRoleRepository.ListAsync(userRolesSpec, cancellationToken);
            
            // Get roles
            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
            var roles = new List<Role>();
            
            foreach (var roleId in roleIds)
            {
                var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
                if (role != null)
                {
                    roles.Add(role);
                }
            }
            
            return _mapper.Map<List<RoleDto>>(roles);
        }
    }
    
    #endregion
}