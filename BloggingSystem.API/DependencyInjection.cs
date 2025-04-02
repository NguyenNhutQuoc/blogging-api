using BloggingSystem.API.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BloggingSystem.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiExceptionFilter(this IServiceCollection services)
    {
        // Đăng ký global exception filter
        services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add<ApiExceptionFilter>();
        });
        
        return services;
    }
}