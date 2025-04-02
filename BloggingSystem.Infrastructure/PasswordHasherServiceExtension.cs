using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BloggingSystem.Infrastructure
{
    public static class PasswordHasherServiceExtensions
    {
        /// <summary>
        /// Add password hasher services to the container
        /// </summary>
        public static IServiceCollection AddPasswordHasher(this IServiceCollection services)
        {
            // Register BCrypt password hasher
            services.AddTransient<IPasswordHasher, BCryptPasswordHasher>();
            
            // Configure password hasher options
            services.Configure<PasswordHasherOptions>(options =>
            {
                options.WorkFactor = 12; // Default value, adjust based on your security requirements
            });
            
            return services;
        }
    }
}