using System.Net.Http.Json;
using AutoMapper;
using MediatR;
using BloggingSystem.Shared.Exceptions;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Features.SocialAuth;
using BloggingSystem.Application.Features.User;
using BloggingSystem.Application.Features.Role;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Shared.DTOs;

namespace BloggingSystem.Application.Authentication.Commands
{
    public class GoogleAuthCommand : IRequest<AuthenticationResult>
    {
        public string AccessToken { get; set; } = null!;
        public string IpAddress { get; set; } = null!;
        public string UserAgent { get; set; } = null!;
    }

    public class GoogleAuthCommandHandler : IRequestHandler<GoogleAuthCommand, AuthenticationResult>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<SocialAuth> _socialAuthRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserSession> _sessionRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMapper _mapper;
        private readonly IDomainEventService _domainEventService;

        public GoogleAuthCommandHandler(
            IRepository<User> userRepository,
            IRepository<SocialAuth> socialAuthRepository,
            IRepository<Role> roleRepository,
            IRepository<UserSession> sessionRepository,
            IRepository<UserRole> userRoleRepository,
            IPasswordHasher passwordHasher,
            IJwtGenerator jwtGenerator,
            IHttpClientFactory httpClientFactory,
            IMapper mapper, IDomainEventService domainEventService)
        {
            _userRepository = userRepository;
            _socialAuthRepository = socialAuthRepository;
            _roleRepository = roleRepository;
            _sessionRepository = sessionRepository;
            _userRoleRepository = userRoleRepository;
            _passwordHasher = passwordHasher;
            _jwtGenerator = jwtGenerator;
            _httpClientFactory = httpClientFactory;
            _mapper = mapper;
            _domainEventService = domainEventService;
        }

        public async Task<AuthenticationResult> Handle(GoogleAuthCommand request, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {request.AccessToken}");

            // Fetch user info from Google
            var response = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo", cancellationToken);
            response.EnsureSuccessStatusCode();

            var googleUser = await response.Content.ReadFromJsonAsync<GoogleUserInfo>(cancellationToken: cancellationToken);
            if (googleUser == null || string.IsNullOrEmpty(googleUser.Email))
                throw new AuthenticationException("Invalid Google user information");

            return await ProcessSocialLoginAsync(
                "google",
                googleUser.Sub,
                googleUser.Email,
                googleUser.Name,
                request.AccessToken,
                null,
                request.IpAddress,
                request.UserAgent,
                cancellationToken);
        }

        private async Task<AuthenticationResult> ProcessSocialLoginAsync(
            string provider,
            string providerUserId,
            string email,
            string name,
            string accessToken,
            string? refreshToken,
            string ipAddress,
            string userAgent,
            CancellationToken cancellationToken)
        {
            // Check if this social login exists
            var socialAuthSpec = new SocialAuthByProviderAndProviderUserIdSpecification(provider, providerUserId);
            var socialAuth = await _socialAuthRepository.FirstOrDefaultAsync(socialAuthSpec, cancellationToken);

            if (socialAuth != null)
            {
                // Update tokens
                socialAuth.Update( accessToken, refreshToken);

                await _socialAuthRepository.UpdateAsync(socialAuth, cancellationToken);
                await _domainEventService.PublishEventsAsync(socialAuth.DomainEvents);

                // Get the user
                var user = socialAuth.User;
                if (user == null)
                    throw new AuthenticationException("User not found");

                if (!user.IsActive)
                    throw new AuthenticationException("User account is not active");

                // Generate authentication response
                return await GenerateAuthResponseAsync(user, ipAddress, userAgent, cancellationToken);
            }
            else
            {
                // Check if user with this email exists
                var userByEmailSpec = new UserByEmailSpecification(email);
                var existingUser = await _userRepository.FirstOrDefaultAsync(userByEmailSpec, cancellationToken);

                if (existingUser != null)
                {
                    // Link social account to existing user
                    socialAuth = SocialAuth.Create(
                        existingUser.Id,
                        provider,
                        providerUserId,
                        accessToken,
                        refreshToken);

                    await _socialAuthRepository.AddAsync(socialAuth, cancellationToken);
                    await _domainEventService.PublishEventsAsync(socialAuth.DomainEvents);

                    return await GenerateAuthResponseAsync(existingUser, ipAddress, userAgent, cancellationToken);
                }
                else
                {
                    // Create new user
                    string username = GenerateUniqueUsername(name);
                    string randomPassword = Guid.NewGuid().ToString();
                    string passwordHash = _passwordHasher.HashPassword(randomPassword);

                    // Create new user
                    var user = User.Create(username, email, passwordHash);
                    user.Activate();

                    await _userRepository.AddAsync(user, cancellationToken);
                    await _domainEventService.PublishEventsAsync(user.DomainEvents);

                    // Assign default user role
                    var roleBySlugSpec = new RoleBySlugSpecification("user");
                    var defaultRole = await _roleRepository.FirstOrDefaultAsync(roleBySlugSpec, cancellationToken);
                    if (defaultRole != null)
                    {
                        var userRole = UserRole.Create(user.Id, defaultRole.Id);
                        await _userRoleRepository.AddAsync(userRole, cancellationToken);
                        await _domainEventService.PublishEventsAsync(userRole.DomainEvents);
                    }

                    // Add social auth record
                    socialAuth = SocialAuth.Create(
                        user.Id,
                        provider,
                        providerUserId,
                        accessToken,
                        refreshToken);

                    await _socialAuthRepository.AddAsync(socialAuth, cancellationToken);
                    await _domainEventService.PublishEventsAsync(socialAuth.DomainEvents);

                    return await GenerateAuthResponseAsync(user, ipAddress, userAgent, cancellationToken);
                }
            }
        }

        private async Task<AuthenticationResult> GenerateAuthResponseAsync(User user, string ipAddress, string userAgent, CancellationToken cancellationToken)
        {
            // Get user roles and permissions
            var userRolesSpec = new RolesWithUserSpecification(user.Id);
            var roles = await _roleRepository.ListAsync(userRolesSpec, cancellationToken);
            var rolesPermission = roles.SelectMany(r => r?.RolePermissions).ToList();
            var permissions = rolesPermission.Select(rp => rp?.Permission).ToList();

            // Generate tokens
            var (accessToken, refreshToken) = _jwtGenerator.GenerateTokens(user, roles, permissions);

            // Create user session
            var session = UserSession.Create(user.Id, refreshToken, ipAddress, userAgent,
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

        private string GenerateUniqueUsername(string name)
        {
            // Create a username from name by removing spaces and adding random suffix
            string baseUsername = name.Replace(" ", "").ToLower();
            
            // Add random suffix to ensure uniqueness
            return $"{baseUsername}_{Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }

    public class GoogleUserInfo
    {
        public string Sub { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Picture { get; set; } = null!;
    }
}