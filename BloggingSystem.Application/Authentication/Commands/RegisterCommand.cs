using AutoMapper;
using MediatR;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Features.Role;
using BloggingSystem.Application.Features.User;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Shared.DTOs;
using BloggingSystem.Domain.Exceptions;

namespace BloggingSystem.Application.Authentication.Commands
{
    public class RegisterCommand : IRequest<AuthenticationResult>
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string IpAddress { get; set; } = null!;
        public string UserAgent { get; set; } = null!;
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthenticationResult>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserSession> _sessionRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IMapper _mapper;
        private readonly IDomainEventService _domainEventService;

        public RegisterCommandHandler(
            IRepository<User> userRepository,
            IRepository<UserSession> sessionRepository,
            IRepository<UserRole> userRoleRepository,
            IRepository<Role> roleRepository,
            IPasswordHasher passwordHasher,
            IJwtGenerator jwtGenerator,
            IMapper mapper, IDomainEventService domainEventService)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _jwtGenerator = jwtGenerator;
            _mapper = mapper;
            _domainEventService = domainEventService;
        }

        public async Task<AuthenticationResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Check if user already exists
            var userByEmailSpec = new UserByEmailSpecification(request.Email);
            var existingEmail = await _userRepository.FirstOrDefaultAsync(userByEmailSpec, cancellationToken);
            if (existingEmail != null)
                throw new DomainException("Email already registered");

            var userByUsernameSpec = new UserByUsernameSpecification(request.Username);
            var existingUsername = await _userRepository.FirstOrDefaultAsync(userByUsernameSpec, cancellationToken);
            if (existingUsername != null)
                throw new DomainException("Username already taken");

            // Hash password
            string passwordHash = _passwordHasher.HashPassword(request.Password);

            // Create new user
            var user = User.Create(request.Username, request.Email, passwordHash);
            user.Activate(); // Auto-activate for now, could be changed to require email verification

            await _userRepository.AddAsync(user, cancellationToken);
            await _domainEventService.PublishEventsAsync(user.DomainEvents);

            // Assign default user role
            var roleBySlugSpec = new RoleBySlugSpecification("user");
            var defaultRole = await _roleRepository.FirstOrDefaultAsync(roleBySlugSpec, cancellationToken);
            if (defaultRole != null)
            {
                var userRole = UserRole.Create(user.Id, defaultRole.Id);
                // In a real app, you would add this user role to the database
                // Here we assume you have a method to do this
                await _userRoleRepository.AddAsync(userRole, cancellationToken);
                await _domainEventService.PublishEventsAsync(userRole.DomainEvents);
            }

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
                    Roles = _mapper.Map<List<RoleDto>>(roles)
                }
            };
        }
    }
}