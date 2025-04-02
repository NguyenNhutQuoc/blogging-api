using AutoMapper;
using MediatR;
using BloggingSystem.Shared.Exceptions;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Features.User;
using BloggingSystem.Application.Features.SocialAuth;
using BloggingSystem.Application.Features.Role;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Shared.DTOs;

namespace BloggingSystem.Application.Users.Queries
{
    public class GetCurrentUserQuery : IRequest<UserDto>
    {
        public long UserId { get; set; }
    }

    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<SocialAuth> _socialAuthRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IMapper _mapper;

        public GetCurrentUserQueryHandler(
            IRepository<User> userRepository,
            IRepository<SocialAuth> socialAuthRepository,
            IRepository<Role> roleRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _socialAuthRepository = socialAuthRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var userSpec = new UserByIdSpecification(request.UserId);
            var user = await _userRepository.FirstOrDefaultAsync(userSpec, cancellationToken);
            
            if (user == null)
                throw new NotFoundException("User not found");

            var userRolesSpec = new RolesWithUserSpecification(request.UserId);
            var roles = await _roleRepository.ListAsync(userRolesSpec, cancellationToken);
            
            var socialAuthSpec = new SocialAuthByUserIdSpecification(request.UserId);
            var socialAuths = await _socialAuthRepository.ListAsync(socialAuthSpec, cancellationToken);

            var UserDto = _mapper.Map<UserDto>(user);
            UserDto.Roles = _mapper.Map<List<RoleDto>>(roles);
            UserDto.SocialAuths = _mapper.Map<List<SocialAuthDto>>(socialAuths);

            return UserDto;
        }
    }
}