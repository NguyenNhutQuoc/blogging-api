using BloggingSystem.Application;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Features.User;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Domain.Events;
using BloggingSystem.Domain.Exceptions;
using BloggingSystem.Shared.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Application.Commands
{
    #region Register User Command

    public class RegisterUserCommand : IRequest<UserDto>
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
    }

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserProfile> _profileRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<RegisterUserCommandHandler> _logger;

        public RegisterUserCommandHandler(
            IRepository<User> userRepository,
            IRepository<UserProfile> profileRepository,
            IPasswordHasher passwordHasher,
            IDomainEventService domainEventService,
            ILogger<RegisterUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _profileRepository = profileRepository;
            _passwordHasher = passwordHasher;
            _domainEventService = domainEventService;
            _logger = logger;
        }

        public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Check if username already exists
            var usernameSpec = new UserByUsernameSpecification(request.Username);
            if (await _userRepository.AnyAsync(usernameSpec, cancellationToken))
            {
                throw new DomainException($"Username '{request.Username}' is already taken");
            }

            // Check if email already exists
            var emailSpec = new UserByEmailSpecification(request.Email);
            if (await _userRepository.AnyAsync(emailSpec, cancellationToken))
            {
                throw new DomainException($"Email '{request.Email}' is already registered");
            }

            // Hash password
            var passwordHash = _passwordHasher.HashPassword(request.Password);

            // Create user
            var user = User.Create(request.Username, request.Email, passwordHash);
            
            // Save user
            await _userRepository.AddAsync(user, cancellationToken);
            
            // Create profile
            var displayName = string.IsNullOrWhiteSpace(request.DisplayName) ? request.Username : request.DisplayName;
            var profile = UserProfile.Create(user.Id, displayName);
            if (!string.IsNullOrWhiteSpace(request.Bio))
            {
                profile.UpdateBasicInfo(displayName, request.Bio, "");
            }
            
            // Save profile
            await _profileRepository.AddAsync(profile, cancellationToken);
            
            // Publish domain events
            await _domainEventService.PublishEventsAsync(user.DomainEvents);
            
            // Return DTO
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                DisplayName = profile.DisplayName,
                Bio = profile.Bio
            };
        }
    }

    #endregion

    #region Activate User Command

    public class ActivateUserCommand : IRequest<bool>
    {
        public long UserId { get; set; }
    }

    public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, bool>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<ActivateUserCommandHandler> _logger;

        public ActivateUserCommandHandler(
            IRepository<User> userRepository,
            IDomainEventService domainEventService,
            ILogger<ActivateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _domainEventService = domainEventService;
            _logger = logger;
        }

        public async Task<bool> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            // Get user by ID
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new EntityNotFoundException(nameof(User), request.UserId);
            }

            // Activate user
            user.Activate();

            // Update user
            await _userRepository.UpdateAsync(user, cancellationToken);

            // Publish domain events
            await _domainEventService.PublishEventsAsync(user.DomainEvents);

            return true;
        }
    }

    #endregion

    #region Deactivate User Command

    public class DeactivateUserCommand : IRequest<bool>
    {
        public long UserId { get; set; }
    }

    public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, bool>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<DeactivateUserCommandHandler> _logger;

        public DeactivateUserCommandHandler(
            IRepository<User> userRepository,
            IDomainEventService domainEventService,
            ILogger<DeactivateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _domainEventService = domainEventService;
            _logger = logger;
        }

        public async Task<bool> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
        {
            // Get user by ID
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new EntityNotFoundException(nameof(User), request.UserId);
            }

            // Deactivate user
            user.Deactivate();

            // Update user
            await _userRepository.UpdateAsync(user, cancellationToken);

            // Publish domain events
            await _domainEventService.PublishEventsAsync(user.DomainEvents);

            return true;
        }
    }

    #endregion

    #region Update Profile Command

    public class UpdateUserProfileCommand : IRequest<UserDto>
    {
        public long UserId { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public string Website { get; set; }
        public string Location { get; set; }
        public string AvatarUrl { get; set; }
    }

    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, UserDto>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserProfile> _profileRepository;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<UpdateUserProfileCommandHandler> _logger;

        public UpdateUserProfileCommandHandler(
            IRepository<User> userRepository,
            IRepository<UserProfile> profileRepository,
            IDomainEventService domainEventService,
            ILogger<UpdateUserProfileCommandHandler> logger)
        {
            _userRepository = userRepository;
            _profileRepository = profileRepository;
            _domainEventService = domainEventService;
            _logger = logger;
        }

        public async Task<UserDto> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            // Get user by ID
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new EntityNotFoundException(nameof(User), request.UserId);
            }

            // Get or create profile
            var profile = await _profileRepository.FirstOrDefaultAsync(
                new ProfileByUserIdSpecification(request.UserId), cancellationToken);

            if (profile == null)
            {
                profile = UserProfile.Create(request.UserId, request.DisplayName);
                await _profileRepository.AddAsync(profile, cancellationToken);
            }
            else
            {
                // Sử dụng các phương thức domain
                profile.UpdateBasicInfo(request.DisplayName, request.Bio, request.Location);
                profile.UpdateWebsite(request.Website);
                profile.UpdateAvatarUrl(request.AvatarUrl);
            }

            // Save changes
            await _profileRepository.UpdateAsync(profile, cancellationToken);

            // Return DTO
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                DisplayName = profile.DisplayName,
                Bio = profile.Bio,
                Website = profile.Website,
                Location = profile.Location,
                AvatarUrl = profile.AvatarUrl
            };
        }
    }

    #endregion

    #region Change Password Command

    public class ChangePasswordCommand : IRequest<bool>
    {
        public long UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;

        public ChangePasswordCommandHandler(
            IRepository<User> userRepository,
            IPasswordHasher passwordHasher,
            IDomainEventService domainEventService,
            ILogger<ChangePasswordCommandHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _domainEventService = domainEventService;
            _logger = logger;
        }

        public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            // Get user by ID
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new EntityNotFoundException(nameof(User), request.UserId);
            }

            // Verify current password
            if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.CurrentPassword))
            {
                throw new DomainException("Current password is incorrect");
            }

            // Hash new password
            var newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);

            // Change password
            user.ChangePassword(newPasswordHash);

            // Update user
            await _userRepository.UpdateAsync(user, cancellationToken);

            // Publish domain events
            await _domainEventService.PublishEventsAsync(user.DomainEvents);

            return true;
        }
    }

    #endregion

    #region Assign Role Command

    public class AssignRoleToUserCommand : IRequest<bool>
    {
        public long UserId { get; set; }
        public long RoleId { get; set; }
    }

    public class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommand, bool>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<AssignRoleToUserCommandHandler> _logger;

        public AssignRoleToUserCommandHandler(
            IRepository<User> userRepository,
            IRepository<Role> roleRepository,
            IRepository<UserRole> userRoleRepository,
            IDomainEventService domainEventService,
            ILogger<AssignRoleToUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _domainEventService = domainEventService;
            _logger = logger;
        }

        public async Task<bool> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
        {
            // Get user by ID
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new EntityNotFoundException(nameof(User), request.UserId);
            }

            // Get role by ID
            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            if (role == null)
            {
                throw new EntityNotFoundException(nameof(Role), request.RoleId);
            }

            // Check if user already has this role
            var userRoleSpec = new UserRoleSpecification(request.UserId, request.RoleId);
            if (await _userRoleRepository.AnyAsync(userRoleSpec, cancellationToken))
            {
                throw new DomainException("User already has this role");
            }

            // Create user role
            var userRole = new UserRole(user.Id, role.Id);

            // Save user role
            await _userRoleRepository.AddAsync(userRole, cancellationToken);

            // Add domain event
            user.AddDomainEvent(new UserRoleAssignedEvent(user.Id, role.Id));

            // Publish domain events
            await _domainEventService.PublishEventsAsync(user.DomainEvents);

            return true;
        }
    }

    #endregion
    
    #region Revoke Role Command
    public class RevokeRoleFromUserCommand : IRequest<bool>
    {
        public long UserId { get; set; }
        public long RoleId { get; set; }
    }

    public class RevokeRoleFromUserCommandHandler : IRequestHandler<RevokeRoleFromUserCommand, bool>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<RevokeRoleFromUserCommandHandler> _logger;
        public RevokeRoleFromUserCommandHandler(
            IRepository<User> userRepository,
            IRepository<Role> roleRepository,
            IRepository<UserRole> userRoleRepository,
            IDomainEventService domainEventService,
            ILogger<RevokeRoleFromUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _domainEventService = domainEventService;
            _logger = logger;
        }
        
        public async Task<bool> Handle(RevokeRoleFromUserCommand request, CancellationToken cancellationToken)
        {
            // Get user by ID
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new EntityNotFoundException(nameof(User), request.UserId);
            }

            // Get role by ID
            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            if (role == null)
            {
                throw new EntityNotFoundException(nameof(Role), request.RoleId);
            }

            // Check if user already has this role
            var userRoleSpec = new UserRoleSpecification(request.UserId, request.RoleId);
            var userRole = await _userRoleRepository.FirstOrDefaultAsync(userRoleSpec, cancellationToken);
            if (userRole == null)
            {
                throw new DomainException("User does not have this role");
            }

            // Remove user role
            await _userRoleRepository.DeleteAsync(userRole, cancellationToken);

            // Add domain event
            user.AddDomainEvent(new UserRoleRemovedEvent(user.Id, role.Id));

            // Publish domain events
            await _domainEventService.PublishEventsAsync(user.DomainEvents);

            return true;
        }
    }
    #endregion
}