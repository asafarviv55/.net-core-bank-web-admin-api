using Microsoft.AspNetCore.Mvc;
using WebAdminAPI.Models;

namespace WebAdminAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private static readonly List<Notification> _notifications = new()
        {
            new Notification
            {
                NotificationId = 1,
                Title = "System Maintenance Scheduled",
                Message = "Scheduled system maintenance on Sunday, 2:00 AM - 4:00 AM",
                NotificationType = "System",
                Priority = "High",
                CreatedDate = DateTime.Now.AddHours(-3),
                TargetUserRole = "All",
                TargetUsers = Array.Empty<string>(),
                IsRead = false,
                IsActive = true
            },
            new Notification
            {
                NotificationId = 2,
                Title = "New Compliance Policy",
                Message = "Updated AML compliance policy effective immediately. Please review.",
                NotificationType = "Compliance",
                Priority = "High",
                CreatedDate = DateTime.Now.AddDays(-1),
                TargetUserRole = "Manager",
                TargetUsers = new[] { "manager1", "manager2" },
                IsRead = true,
                ReadDate = DateTime.Now.AddHours(-12),
                IsActive = true
            },
            new Notification
            {
                NotificationId = 3,
                Title = "Transaction Limit Update",
                Message = "Daily transaction limit has been increased to $50,000",
                NotificationType = "Policy",
                Priority = "Medium",
                CreatedDate = DateTime.Now.AddDays(-2),
                TargetUserRole = "Teller",
                TargetUsers = Array.Empty<string>(),
                IsRead = false,
                IsActive = true
            }
        };

        private readonly ILogger<NotificationController> _logger;

        public NotificationController(ILogger<NotificationController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Notification>> GetAllNotifications([FromQuery] bool? activeOnly = true)
        {
            var notifications = activeOnly == true
                ? _notifications.Where(n => n.IsActive)
                : _notifications;

            return Ok(notifications.OrderByDescending(n => n.CreatedDate));
        }

        [HttpGet("{notificationId}")]
        [ProducesResponseType(typeof(Notification), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Notification> GetNotificationById(int notificationId)
        {
            var notification = _notifications.FirstOrDefault(n => n.NotificationId == notificationId);
            if (notification == null)
            {
                return NotFound($"Notification {notificationId} not found");
            }

            return Ok(notification);
        }

        [HttpGet("user/{username}")]
        [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Notification>> GetNotificationsByUser(string username, [FromQuery] string? userRole = null)
        {
            var notifications = _notifications
                .Where(n => n.IsActive &&
                           (n.TargetUserRole == "All" ||
                            (userRole != null && n.TargetUserRole.Equals(userRole, StringComparison.OrdinalIgnoreCase)) ||
                            n.TargetUsers.Contains(username, StringComparer.OrdinalIgnoreCase)))
                .OrderByDescending(n => n.CreatedDate);

            return Ok(notifications);
        }

        [HttpGet("unread/{username}")]
        [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Notification>> GetUnreadNotifications(string username, [FromQuery] string? userRole = null)
        {
            var notifications = _notifications
                .Where(n => n.IsActive &&
                           !n.IsRead &&
                           (n.TargetUserRole == "All" ||
                            (userRole != null && n.TargetUserRole.Equals(userRole, StringComparison.OrdinalIgnoreCase)) ||
                            n.TargetUsers.Contains(username, StringComparer.OrdinalIgnoreCase)))
                .OrderByDescending(n => n.CreatedDate);

            return Ok(notifications);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Notification), StatusCodes.Status201Created)]
        public ActionResult<Notification> CreateNotification([FromBody] Notification notification)
        {
            notification.NotificationId = _notifications.Any() ? _notifications.Max(n => n.NotificationId) + 1 : 1;
            notification.CreatedDate = DateTime.Now;
            notification.IsRead = false;
            notification.IsActive = true;
            _notifications.Add(notification);

            _logger.LogInformation($"Created notification {notification.NotificationId}: {notification.Title}");
            return CreatedAtAction(nameof(GetNotificationById),
                new { notificationId = notification.NotificationId }, notification);
        }

        [HttpPut("{notificationId}/read")]
        [ProducesResponseType(typeof(Notification), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Notification> MarkAsRead(int notificationId)
        {
            var notification = _notifications.FirstOrDefault(n => n.NotificationId == notificationId);
            if (notification == null)
            {
                return NotFound($"Notification {notificationId} not found");
            }

            notification.IsRead = true;
            notification.ReadDate = DateTime.Now;
            _logger.LogInformation($"Marked notification {notificationId} as read");
            return Ok(notification);
        }

        [HttpDelete("{notificationId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteNotification(int notificationId)
        {
            var notification = _notifications.FirstOrDefault(n => n.NotificationId == notificationId);
            if (notification == null)
            {
                return NotFound($"Notification {notificationId} not found");
            }

            notification.IsActive = false;
            _logger.LogInformation($"Deleted notification {notificationId}");
            return NoContent();
        }

        [HttpGet("statistics")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public ActionResult<object> GetNotificationStatistics()
        {
            var stats = new
            {
                TotalActive = _notifications.Count(n => n.IsActive),
                UnreadCount = _notifications.Count(n => n.IsActive && !n.IsRead),
                ReadCount = _notifications.Count(n => n.IsActive && n.IsRead),
                ByPriority = _notifications
                    .Where(n => n.IsActive)
                    .GroupBy(n => n.Priority)
                    .ToDictionary(g => g.Key, g => g.Count()),
                ByType = _notifications
                    .Where(n => n.IsActive)
                    .GroupBy(n => n.NotificationType)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            return Ok(stats);
        }
    }
}
