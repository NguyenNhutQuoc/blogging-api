using System.Threading.Tasks;

namespace BloggingSystem.Application.Commons.Interfaces
{
    /// <summary>
    /// Interface for audit log service
    /// </summary>
    public interface IAuditLogService
    {
        /// <summary>
        /// Add an audit log entry
        /// </summary>
        /// <param name="userId">ID of the user performing the action (optional)</param>
        /// <param name="action">Action performed (create, update, delete, etc.)</param>
        /// <param name="entityType">Type of entity affected</param>
        /// <param name="entityId">ID of entity affected</param>
        /// <param name="oldValues">Old values (for updates)</param>
        /// <param name="newValues">New values (for creates/updates)</param>
        /// <param name="ipAddress">IP address of the user (optional)</param>
        /// <param name="userAgent">User agent string (optional)</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task AddAuditLogAsync(
            long? userId,
            string action,
            string entityType,
            long entityId,
            object oldValues = null,
            object newValues = null,
            string ipAddress = null,
            string userAgent = null);
    }
}