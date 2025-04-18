using System.IO.Compression;
using BloggingSystem.API.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;

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
    
    public static IServiceCollection AddCompressionServices(this IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.SmallestSize;
        });

        return services;
    }
}