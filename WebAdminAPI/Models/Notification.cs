namespace WebAdminAPI.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string TargetUserRole { get; set; } = string.Empty;
        public string[] TargetUsers { get; set; } = Array.Empty<string>();
        public bool IsRead { get; set; }
        public DateTime? ReadDate { get; set; }
        public bool IsActive { get; set; }
    }
}
