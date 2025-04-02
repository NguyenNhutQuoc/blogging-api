using AutoMapper;
using MediatR;
using BloggingSystem.Shared.Exceptions;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Features.User;
using BloggingSystem.Application.Features.Role;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Shared.DTOs;

namespace BloggingSystem.Application.Users.Queries
{
    public class GetUserRolesQuery : IRequest<List<RoleDto>>
    {
        public long UserId { get; set; }
    }

    public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, List<RoleDto>>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IMapper _mapper;

        public GetUserRolesQueryHandler(
            IRepository<User> userRepository,
            IRepository<Role> roleRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<List<RoleDto>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            var userSpec = new UserByIdSpecification(request.UserId);
            var user = await _userRepository.FirstOrDefaultAsync(userSpec, cancellationToken);
            
            if (user == null)
                throw new NotFoundException("User not found");

            var userRolesSpec = new RolesWithUserSpecification(request.UserId);
            var roles = await _roleRepository.ListAsync(userRolesSpec, cancellationToken);

            return _mapper.Map<List<RoleDto>>(roles);
        }
    }
}