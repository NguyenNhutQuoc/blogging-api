using System;
using System.Text.Json;
using System.Threading.Tasks;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Infrastructure.Services
{
    /// <summary>
    /// Implementation of audit log service
    /// </summary>
    public class AuditLogService : IAuditLogService
    {
        private readonly IRepository<AuditLog> _auditLogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(
            IRepository<AuditLog> auditLogRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuditLogService> logger)
        {
            _auditLogRepository = auditLogRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// Add an audit log entry
        /// </summary>
        public async Task AddAuditLogAsync(
            long? userId,
            string action,
            string entityType,
            long entityId,
            object oldValues = null,
            object newValues = null,
            string ipAddress = null,
            string userAgent = null)
        {
            // Get IP address and user agent from current HTTP context if not provided
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                ipAddress ??= httpContext.Connection.RemoteIpAddress?.ToString();
                userAgent ??= httpContext.Request.Headers["User-Agent"].ToString();
            }

            // Serialize objects to JSON
            string oldValuesJson = null;
            string newValuesJson = null;

            if (oldValues != null)
                oldValuesJson = JsonSerializer.Serialize(oldValues);

            if (newValues != null)
                newValuesJson = JsonSerializer.Serialize(newValues);

            // Create audit log entry
            var auditLog = AuditLog.Create(
                userId,
                action,
                entityType,
                entityId,
                oldValuesJson,
                newValuesJson,
                ipAddress,
                userAgent);

            // Save audit log to the database (or any other storage mechanism you choose to use for audit logs, such as a database, file, etc.)

            try
            {
                await _auditLogRepository.AddAsync(auditLog);
                _logger.LogDebug(
                    "Added audit log: User {UserId}, Action {Action}, Entity {EntityType}:{EntityId}",
                    userId, action, entityType, entityId);
            }
            catch (Exception ex)
            {
                // Log error but don't fail the operation
                _logger.LogError(ex, 
                    "Failed to add audit log: User {UserId}, Action {Action}, Entity {EntityType}:{EntityId}",
                    userId, action, entityType, entityId);
            }
        }
    }
}