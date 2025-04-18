using AutoMapper;
using MediatR;
using BloggingSystem.Shared.Exceptions;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Features.UserSession;
using BloggingSystem.Application.Features.Role;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Shared.DTOs;

namespace BloggingSystem.Application.Authentication.Commands
{
    public class RefreshTokenCommand : IRequest<AuthenticationResult>
    {
        public string RefreshToken { get; set; } = null!;
        public string IpAddress { get; set; } = null!;
        public string UserAgent { get; set; } = null!;
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthenticationResult>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserSession> _sessionRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IMapper _mapper;
        private readonly IDomainEventService _domainEventService;

        public RefreshTokenCommandHandler(
            IRepository<User> userRepository,
            IRepository<UserSession> sessionRepository,
            IRepository<Role> roleRepository,
            IJwtGenerator jwtGenerator,
            IMapper mapper, IDomainEventService domainEventService)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _roleRepository = roleRepository;
            _jwtGenerator = jwtGenerator;
            _mapper = mapper;
            _domainEventService = domainEventService;
        }   

        public async Task<AuthenticationResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // Find the session by refresh token
            var sessionByTokenSpec = new UserSessionByTokenSpecification(request.RefreshToken);
            var session = await _sessionRepository.FirstOrDefaultAsync(sessionByTokenSpec, cancellationToken);
            if (session == null || session.ExpiresAt < DateTime.UtcNow)
                throw new AuthenticationException("Invalid or expired refresh token");

            // Get the user
            var user = session.User;
            if (user == null || !user.IsActive)
                throw new AuthenticationException("User not found or inactive");

            // Get user roles and permissions
            var userRolesSpec = new RolesWithUserSpecification(user.Id);
            var roles = await _roleRepository.ListAsync(userRolesSpec, cancellationToken);
            var rolesPermission = roles.SelectMany(r => r?.RolePermissions).ToList();
            var permissions = rolesPermission.Select(rp => rp?.Permission).ToList();

            // Generate new tokens
            var (accessToken, newRefreshToken) = _jwtGenerator.GenerateTokens(user, roles, permissions);

            // Update session
            session.Update(newRefreshToken, request.IpAddress, request.UserAgent, DateTime.UtcNow.AddDays(7));

            await _sessionRepository.UpdateAsync(session, cancellationToken);
            
            // Publish domain event
            await _domainEventService.PublishEventsAsync(session.DomainEvents);

            return new AuthenticationResult
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = 3600, // 1 hour in seconds
                TokenType = "Bearer",
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    Roles = _mapper.Map<List<RoleDto>>(roles)
                }
            };
        }
    }
}