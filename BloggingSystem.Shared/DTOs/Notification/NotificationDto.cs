namespace BloggingSystem.Shared.DTOs.Notification;

/// <summary>
/// Notification DTO
/// </summary>
public class NotificationDto
{
    /// <summary>
    /// Notification type
    /// </summary>
    public string Type { get; set; }
        
    /// <summary>
    /// Notification title
    /// </summary>
    public string Title { get; set; }
        
    /// <summary>
    /// Notification message
    /// </summary>
    public string Message { get; set; }
        
    /// <summary>
    /// Recipient ID
    /// </summary>
    public Guid RecipientId { get; set; }
        
    /// <summary>
    /// Additional data
    /// </summary>
    public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();
}