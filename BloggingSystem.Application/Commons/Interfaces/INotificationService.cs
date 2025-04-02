using BloggingSystem.Shared.DTOs.Notification;

namespace BloggingSystem.Application.Commons.Interfaces;
/// <summary>
/// Notification service interface
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Register notification channel
    /// </summary>
    void RegisterChannel(INotificationChannel channel);
        
    /// <summary>
    /// Unregister notification channel
    /// </summary>
    void UnregisterChannel(INotificationChannel channel);
        
    /// <summary>
    /// Send notification
    /// </summary>
    Task SendNotificationAsync(NotificationDto notification);
}

/// <summary>
/// Notification channel interface
/// </summary>
public interface INotificationChannel
{
    /// <summary>
    /// Send notification
    /// </summary>
    Task SendAsync(NotificationDto notification);
        
    /// <summary>
    /// Check if channel can handle notification
    /// </summary>
    bool CanHandle(NotificationDto notification);
}