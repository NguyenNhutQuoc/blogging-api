using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Infrastructure.Authorization;
using BloggingSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BloggingSystem.Infrastructure.Authentication
{
    public static class AuthenticationSetup
    {
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure JWT authentication
            var jwtSection = configuration.GetSection("JwtSettings");
            services.Configure<JwtSettings>(jwtSection);

            var jwtSettings = jwtSection.Get<JwtSettings>()!;
            var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Set to true in production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero // Remove delay of token expiration
                };
                
                // Add event handling for token validation errors
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Append("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            // Configure social auth
            var socialAuthSection = configuration.GetSection("SocialAuthSettings");
            services.Configure<SocialAuthSettings>(socialAuthSection);

            // Register services
            services.AddSingleton<IJwtGenerator, JwtGenerator>();
            // Register HttpClient factory for social auth
            services.AddHttpClient();

            return services;
        }

        public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy =>
                    policy.RequireRole("admin"));

                // Register policies for all available permissions
                var permissions = new[]
                {
                    "user.create", "user.read", "user.update", "user.delete",
                    "role.create", "role.read", "role.update", "role.delete",
                    "post.create", "post.read", "post.edit", "post.delete", "post.publish", "post.unpublish", "post.read-all",
                    "comment.create", "comment.read", "comment.update", "comment.delete",
                    "category.create", "category.read", "category.update", "category.delete",
                    "tag.create", "tag.read", "tag.update", "tag.delete",
                    "settings.update", "settings.read"
                };

                foreach (var permission in permissions)
                {
                    options.AddPolicy($"Permission:{permission}", policy =>
                        policy.Requirements.Add(new PermissionRequirement(permission)));
                }
                
                options.AddPolicy("Permission:post.update", policy =>
                    policy.Requirements.Add(new PostAuthorRequirement("post.update")));
                options.AddPolicy("Permission:post.delete", policy =>
                    policy.Requirements.Add(new PostAuthorRequirement("post.delete")));
                options.AddPolicy("Permission:post.unpublish", policy =>
                    policy.Requirements.Add(new PostAuthorRequirement("post.unpublish")));
                options.AddPolicy("Permission:post.publish", policy =>
                    policy.Requirements.Add(new PostPublishedRequirement("post.publish")));
            });

            return services;
        }
    }

    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            // Get all permission claims
            var permissions = context.User.Claims
                .Where(c => c.Type == "permissions")
                .Select(c => c.Value)
                .ToList();

            // Check if user has the required permission
            if (permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}