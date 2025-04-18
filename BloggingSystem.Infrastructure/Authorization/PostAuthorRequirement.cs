using System.Security.Claims;
using System.Threading.Tasks;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BloggingSystem.Infrastructure.Authorization
{
    // Requirement cho author của post
    public class PostAuthorRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PostAuthorRequirement(string permission = null)
        {
            Permission = permission;
        }
    }

    // Handler xử lý requirement
    public class PostAuthorAuthorizationHandler : AuthorizationHandler<PostAuthorRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<Post> _postRepository;
        private readonly ICurrentUserService _currentUserService;

        public PostAuthorAuthorizationHandler(
            IHttpContextAccessor httpContextAccessor,
            IRepository<Post> postRepository,
            ICurrentUserService currentUserService)
        {
            _httpContextAccessor = httpContextAccessor;
            _postRepository = postRepository;
            _currentUserService = currentUserService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PostAuthorRequirement requirement)
        {
            // Nếu không phải request HTTP, bỏ qua
            if (_httpContextAccessor.HttpContext == null)
            {
                return;
            }

            // Nếu user không được xác thực, bỏ qua
            if (!_currentUserService.IsAuthenticated)
            {
                return;
            }

            // Kiểm tra nếu user là Admin, cho phép luôn
            if (_currentUserService.IsInRole("admin"))
            {
                context.Succeed(requirement);
                return;
            }

            // Kiểm tra permission nếu có
            if (!string.IsNullOrEmpty(requirement.Permission) && 
                _currentUserService.HasPermission(requirement.Permission))
            {
                context.Succeed(requirement);
                return;
            }

            // Lấy postId từ route data
            var routeData = _httpContextAccessor.HttpContext.GetRouteData();
            if (!routeData.Values.TryGetValue("id", out var postIdObj) &&
                !routeData.Values.TryGetValue("postId", out postIdObj))
            {
                // Không tìm thấy postId trong route
                return;
            }

            // Chuyển đổi postId
            if (!long.TryParse(postIdObj?.ToString(), out var postId))
            {
                return;
            }

            // Kiểm tra xem user có phải là author của post
            var post = await _postRepository.GetByIdAsync(postId);
            if (post != null && post.AuthorId == _currentUserService.UserId)
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
    
    
    // Published Post: Just admin or editor can unpublish. Other users can only view, even if they are author.
    public class PostPublishedRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PostPublishedRequirement(string permission = null)
        {
            Permission = permission;
        }
    }
    // Handler xử lý requirement
    public class PostPublishedAuthorizationHandler : AuthorizationHandler<PostPublishedRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserService _currentUserService;
        public PostPublishedAuthorizationHandler(
            IHttpContextAccessor httpContextAccessor,
            ICurrentUserService currentUserService)
        {
            _httpContextAccessor = httpContextAccessor;
            _currentUserService = currentUserService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PostPublishedRequirement requirement)
        {
            // Nếu không phải request HTTP, bỏ qua
            if (_httpContextAccessor.HttpContext == null)
            {
                return;
            }

            // Nếu user không được xác thực, bỏ qua
            if (!_currentUserService.IsAuthenticated)
            {
                return;
            }

            // Kiểm tra nếu user là Admin hoặc Editor, cho phép luôn
            if (_currentUserService.IsInRole("admin") || _currentUserService.IsInRole("editor"))
            {
                context.Succeed(requirement);
                return;
            }

            // Lấy postId từ route data
            var routeData = _httpContextAccessor.HttpContext.GetRouteData();
            if (!routeData.Values.TryGetValue("id", out var postIdObj) &&
                !routeData.Values.TryGetValue("postId", out postIdObj))
            {
                // Không tìm thấy postId trong route
                return;
            }

            // Chuyển đổi postId
            if (!long.TryParse(postIdObj?.ToString(), out var postId))
            {
                return;
            }
        }
    }
}