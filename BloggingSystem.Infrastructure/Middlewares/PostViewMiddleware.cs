using System;
using System.Threading.Tasks;
using BloggingSystem.Application.Features.Post.Command;
using BloggingSystem.Domain.Exceptions;
using BloggingSystem.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Infrastructure.Middlewares
{
    public class PostViewMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PostViewMiddleware> _logger;

        public PostViewMiddleware(RequestDelegate next, ILogger<PostViewMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only process GET requests to post endpoints
            if (context.Request.Method != "GET")
            {
                await _next(context);
                return;
            }

            // Check if this is a post detail request
            var path = context.Request.Path.Value;
            
            // Option 1: Check for path pattern /api/v{version}/posts/by-slug/{id}
            if (path != null && path.Contains("/api/v") && path.Contains("/posts/by-slug/"))
            {
                // For slug-based URLs, we'll need to get the post ID later
                var slug = path.Substring(path.LastIndexOf('/') + 1);
                if (!string.IsNullOrEmpty(slug))
                {
                    // Use the original request to continue processing
                    // The post ID will be resolved by the actual handler and view count will be incremented there
                    context.Items["TrackViewForSlug"] = slug;
                    // You can also log or handle the slug here if needed
                    _logger.LogInformation("Tracking view for post slug: {Slug}", slug);
                    await IncrementPostViewsAsync(context, slug);
                    
                }
            }
            
            // Option 2: Check for path pattern /api/v{version}/posts/{id}
            else if (path != null && path.Contains("/api/v") && path.Contains("/posts/"))
            {
                var segments = path.Split('/');
                if (segments.Length >= 5)
                {
                    var idSegment = segments[4];
                    if (long.TryParse(idSegment, out var postId))
                    {
                        await IncrementPostViewsAsync(context, postId);
                    }
                }
            }

            // Continue processing the request
            await _next(context);
        }

        private async Task IncrementPostViewsAsync(HttpContext context, long postId)
        {
            try
            {
                // Get IP address and user agent to prevent duplicate counts
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var userAgent = context.Request.Headers["User-Agent"].ToString();
                
                // Create hash of visitor info to avoid counting multiple views from same user in short period
                var visitorHash = $"{ipAddress}_{userAgent}";

                // Get mediator from service provider
                var mediator = context.RequestServices.GetRequiredService<IMediator>();

                // Send the command to increment views
                var command = new IncrementPostViewsCommand()
                { 
                    PostId = postId,
                    VisitorHash = visitorHash
                };

                await mediator.Send(command);
            }
            catch (NotFoundException)
            {
                // Post not found, but we'll let the actual request handler deal with it
                _logger.LogWarning("Attempted to increment views for non-existent post ID: {PostId}", postId);
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the request
                _logger.LogError(ex, "Error incrementing views for post ID: {PostId}", postId);
            }
        }
        
        private async Task IncrementPostViewsAsync(HttpContext context, string slug)
        {
            try
            {
                // Get IP address and user agent to prevent duplicate counts
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var userAgent = context.Request.Headers["User-Agent"].ToString();
                
                // Create hash of visitor info to avoid counting multiple views from same user in short period
                var visitorHash = $"{ipAddress}_{userAgent}";

                // Get mediator from service provider
                var mediator = context.RequestServices.GetRequiredService<IMediator>();

                // Send the command to increment views
                var command = new IncrementPostViewsCommand()
                { 
                    Slug = slug,
                    PostId = 0,
                    VisitorHash = visitorHash
                };

                await mediator.Send(command);
            }
            catch (NotFoundException)
            {
                // Post not found, but we'll let the actual request handler deal with it
                _logger.LogWarning("Attempted to increment views for non-existent post slug: {Slug}", slug);
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the request
                _logger.LogError(ex, "Error incrementing views for post slug: {Slug}", slug);
            }
        }
    }
    
    

    // Extension method to add the middleware to the HTTP request pipeline
    public static class PostViewMiddlewareExtensions
    {
        public static IApplicationBuilder UsePostViewTracking(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PostViewMiddleware>();
        }
    }
}