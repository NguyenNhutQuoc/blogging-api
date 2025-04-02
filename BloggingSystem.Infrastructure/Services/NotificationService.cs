using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Shared.DTOs.Notification;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Infrastructure.Services
{
    /// <summary>
    /// Notification service implementation using observer pattern
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly List<INotificationChannel> _channels = new List<INotificationChannel>();
        
        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }
        
        /// <summary>
        /// Register notification channel
        /// </summary>
        public void RegisterChannel(INotificationChannel channel)
        {
            if (!_channels.Contains(channel))
            {
                _channels.Add(channel);
                _logger.LogInformation("Notification channel registered: {channel}", channel.GetType().Name);
            }
        }
        
        /// <summary>
        /// Unregister notification channel
        /// </summary>
        public void UnregisterChannel(INotificationChannel channel)
        {
            if (_channels.Contains(channel))
            {
                _channels.Remove(channel);
                _logger.LogInformation("Notification channel unregistered: {channel}", channel.GetType().Name);
            }
        }
        
        /// <summary>
        /// Send notification to all registered channels
        /// </summary>
        public async Task SendNotificationAsync(NotificationDto notification)
        {
            _logger.LogInformation("Sending notification: {type} to {recipient}", notification.Type, notification.RecipientId);
            
            foreach (var channel in _channels)
            {
                if (channel.CanHandle(notification))
                {
                    try
                    {
                        await channel.SendAsync(notification);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sending notification via {channel}", channel.GetType().Name);
                    }
                }
            }
        }
    }
}