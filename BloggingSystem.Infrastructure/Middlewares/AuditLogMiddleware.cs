using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Domain.Commons;
using BloggingSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IO;

namespace BloggingSystem.Infrastructure.Middlewares
{
    public class AuditLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditLogMiddleware> _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private readonly AuditLogOptions _options;

        public AuditLogMiddleware(
            RequestDelegate next,
            ILogger<AuditLogMiddleware> logger,
            IOptions<AuditLogOptions> options)
        {
            _next = next;
            _logger = logger;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context, IAuditLogService auditLogService, ICurrentUserService currentUserService)
        {
            // Don't log if path starts with excluded prefixes
            foreach (var prefix in _options.ExcludedPathPrefixes)
            {
                if (context.Request.Path.StartsWithSegments(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    await _next(context);
                    return;
                }
            }

            // Skip if not a relevant HTTP method
            if (!_options.MethodsToLog.Contains(context.Request.Method))
            {
                await _next(context);
                return;
            }

            // Get the request body
            var requestBody = await GetRequestBody(context.Request);

            // Create a copy of the response body stream
            var originalBodyStream = context.Response.Body;
            await using var responseBodyStream = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBodyStream;

            // Continue processing the request
            var timestamp = DateTime.UtcNow;
            string responseBody = null;
            
            try
            {
                await _next(context);

                // Get the response body
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                await responseBodyStream.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred during request processing");
                responseBody = $"Error: {ex.Message}";
                throw;
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }

            try
            {
                // Only log actions that result in specific status codes (if configured)
                if (_options.StatusCodesToLog.Count > 0 && 
                    !_options.StatusCodesToLog.Contains(context.Response.StatusCode))
                {
                    return;
                }

                // Prepare the audit log entry
                var auditLog = new AuditLogEntry
                {
                    Timestamp = timestamp,
                    UserId = currentUserService.UserId,
                    UserName = currentUserService.Username,
                    IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = context.Request.Headers["User-Agent"].ToString(),
                    HttpMethod = context.Request.Method,
                    Path = context.Request.Path,
                    QueryString = context.Request.QueryString.ToString(),
                    RequestBody = _options.IncludeRequestBody ? requestBody : null,
                    ResponseBody = _options.IncludeResponseBody ? responseBody : null,
                    StatusCode = context.Response.StatusCode,
                    Duration = (DateTime.UtcNow - timestamp).TotalMilliseconds
                };

                // Save the audit log
                await auditLogService.AddAuditLogAsync(
                    auditLog.UserId,
                    $"{context.Request.Method} {context.Request.Path}",
                    "Request",
                    0, // Entity ID is not applicable for request logs
                    null,
                    JsonSerializer.Serialize(auditLog),
                    auditLog.IpAddress,
                    auditLog.UserAgent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging the request");
            }
        }

        private async Task<string> GetRequestBody(HttpRequest request)
        {
            request.EnableBuffering();

            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await request.Body.CopyToAsync(requestStream);
            
            request.Body.Seek(0, SeekOrigin.Begin);
            
            return Encoding.UTF8.GetString(requestStream.ToArray());
        }
    }

    public class AuditLogOptions
    {
        public List<string> ExcludedPathPrefixes { get; set; } = new List<string> { "/swagger", "/health", "/metrics" };
        public List<string> MethodsToLog { get; set; } = new List<string> { "POST", "PUT", "DELETE", "PATCH" };
        public List<int> StatusCodesToLog { get; set; } = new List<int>();
        public bool IncludeRequestBody { get; set; } = true;
        public bool IncludeResponseBody { get; set; } = true;
        public string LogFilePath { get; set; } = "logs/audit-logs.txt";
    }

    // Extension method to add the middleware to the HTTP request pipeline
    public static class AuditLogMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuditLogging(
            this IApplicationBuilder builder,
            Action<AuditLogOptions> configureOptions = null)
        {
            var options = new AuditLogOptions();
            configureOptions?.Invoke(options);

            return builder.UseMiddleware<AuditLogMiddleware>(Options.Create(options));
        }
    }

    public class AuditLogEntry
    {
        public DateTime Timestamp { get; set; }
        public long? UserId { get; set; }
        public string UserName { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string HttpMethod { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public int StatusCode { get; set; }
        public double Duration { get; set; }
    }
}