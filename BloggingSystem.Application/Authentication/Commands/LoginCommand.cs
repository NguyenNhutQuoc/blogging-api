using AutoMapper;
using MediatR;
using BloggingSystem.Shared.Exceptions;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Features.Role;
using BloggingSystem.Application.Features.User;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Shared.DTOs;

namespace BloggingSystem.Application.Authentication.Commands
{
    public class LoginCommand : IRequest<AuthenticationResult>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string IpAddress { get; set; } = null!;
        public string UserAgent { get; set; } = null!;
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticationResult>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserSession> _sessionRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IMapper _mapper;
        private readonly IDomainEventService _domainEventService;

        public LoginCommandHandler(
            IRepository<User> userRepository,
            IRepository<UserSession> sessionRepository,
            IRepository<Role> roleRepository,
            IPasswordHasher passwordHasher,
            IJwtGenerator jwtGenerator, IMapper mapper, IDomainEventService domainEventService)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _jwtGenerator = jwtGenerator;
            _mapper = mapper;
            _domainEventService = domainEventService;
        }

        public async Task<AuthenticationResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Find user by email
            var findByUserEmailSpec = new UserByEmailSpecification(request.Email);
            var user = await _userRepository.FirstOrDefaultAsync(findByUserEmailSpec, cancellationToken);
            if (user == null)
                throw new AuthenticationException("Invalid credentials");

            // Check if user is active
            if (!user.IsActive)
                throw new AuthenticationException("User account is not active");

            // Verify password
            if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
                throw new AuthenticationException("Invalid credentials");

            // Get user roles and permissions
            var userRolesSpec = new RolesWithUserSpecification(user.Id);

            var roles = await _roleRepository.ListAsync(userRolesSpec, cancellationToken);
            var rolesPermission = roles.SelectMany(r => r?.RolePermissions).ToList();
            var permissions = rolesPermission.Select(rp => rp?.Permission).ToList();

            // Generate tokens
            var (accessToken, refreshToken) = _jwtGenerator.GenerateTokens(user, roles, permissions);

            // Create user session
            var session = UserSession.Create(user.Id, refreshToken, request.IpAddress, request.UserAgent,
                DateTime.UtcNow.AddDays(7));

            await _sessionRepository.AddAsync(session, cancellationToken); 
            await _domainEventService.PublishEventsAsync(session.DomainEvents);
            return new AuthenticationResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = 3600, // 1 hour in seconds
                TokenType = "Bearer",
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    Roles = _mapper.Map<List<RoleDto>>(roles),
                }
            };
        }
    }
}