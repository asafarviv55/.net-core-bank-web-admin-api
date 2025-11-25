using Microsoft.AspNetCore.Mvc;
using WebAdminAPI.Models;

namespace WebAdminAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditController : ControllerBase
    {
        private static readonly List<AuditLog> _auditLogs = new()
        {
            new AuditLog
            {
                LogId = 1,
                Timestamp = DateTime.Now.AddHours(-2),
                UserId = "admin",
                Username = "System Administrator",
                Action = "CREATE",
                EntityType = "Account",
                EntityId = "ACC1001",
                IpAddress = "192.168.1.100",
                Details = "Created new savings account",
                IsSuccessful = true
            },
            new AuditLog
            {
                LogId = 2,
                Timestamp = DateTime.Now.AddHours(-1),
                UserId = "manager1",
                Username = "Branch Manager",
                Action = "APPROVE",
                EntityType = "Transaction",
                EntityId = "TXN001",
                IpAddress = "192.168.1.101",
                Details = "Approved transaction for $1,500",
                IsSuccessful = true
            },
            new AuditLog
            {
                LogId = 3,
                Timestamp = DateTime.Now.AddMinutes(-30),
                UserId = "teller1",
                Username = "Bank Teller",
                Action = "UPDATE",
                EntityType = "Customer",
                EntityId = "CUST001",
                IpAddress = "192.168.1.102",
                Details = "Updated customer phone number",
                IsSuccessful = true
            },
            new AuditLog
            {
                LogId = 4,
                Timestamp = DateTime.Now.AddMinutes(-15),
                UserId = "teller1",
                Username = "Bank Teller",
                Action = "DELETE",
                EntityType = "Transaction",
                EntityId = "TXN999",
                IpAddress = "192.168.1.102",
                Details = "Attempted to delete completed transaction",
                IsSuccessful = false,
                ErrorMessage = "Cannot delete completed transaction"
            }
        };

        private readonly ILogger<AuditController> _logger;

        public AuditController(ILogger<AuditController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AuditLog>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<AuditLog>> GetAllLogs([FromQuery] int? limit = 100)
        {
            _logger.LogInformation("Retrieving audit logs");
            return Ok(_auditLogs.OrderByDescending(l => l.Timestamp).Take(limit.Value));
        }

        [HttpGet("{logId}")]
        [ProducesResponseType(typeof(AuditLog), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<AuditLog> GetLogById(int logId)
        {
            var log = _auditLogs.FirstOrDefault(l => l.LogId == logId);
            if (log == null)
            {
                return NotFound($"Audit log {logId} not found");
            }

            return Ok(log);
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<AuditLog>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<AuditLog>> GetLogsByUser(string userId)
        {
            var logs = _auditLogs
                .Where(l => l.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(l => l.Timestamp);

            return Ok(logs);
        }

        [HttpGet("entity/{entityType}/{entityId}")]
        [ProducesResponseType(typeof(IEnumerable<AuditLog>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<AuditLog>> GetLogsByEntity(string entityType, string entityId)
        {
            var logs = _auditLogs
                .Where(l => l.EntityType.Equals(entityType, StringComparison.OrdinalIgnoreCase) &&
                           l.EntityId.Equals(entityId, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(l => l.Timestamp);

            return Ok(logs);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<AuditLog>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<AuditLog>> SearchLogs(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? action,
            [FromQuery] string? entityType)
        {
            var query = _auditLogs.AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(l => l.Timestamp >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(l => l.Timestamp <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(action))
            {
                query = query.Where(l => l.Action.Equals(action, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(entityType))
            {
                query = query.Where(l => l.EntityType.Equals(entityType, StringComparison.OrdinalIgnoreCase));
            }

            return Ok(query.OrderByDescending(l => l.Timestamp).ToList());
        }

        [HttpPost]
        [ProducesResponseType(typeof(AuditLog), StatusCodes.Status201Created)]
        public ActionResult<AuditLog> CreateAuditLog([FromBody] AuditLog log)
        {
            log.LogId = _auditLogs.Any() ? _auditLogs.Max(l => l.LogId) + 1 : 1;
            log.Timestamp = DateTime.Now;
            _auditLogs.Add(log);

            _logger.LogInformation($"Created audit log {log.LogId}");
            return CreatedAtAction(nameof(GetLogById), new { logId = log.LogId }, log);
        }

        [HttpGet("statistics")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public ActionResult<object> GetAuditStatistics()
        {
            var stats = new
            {
                TotalLogs = _auditLogs.Count,
                SuccessfulActions = _auditLogs.Count(l => l.IsSuccessful),
                FailedActions = _auditLogs.Count(l => !l.IsSuccessful),
                LogsToday = _auditLogs.Count(l => l.Timestamp.Date == DateTime.Today),
                ActionBreakdown = _auditLogs
                    .GroupBy(l => l.Action)
                    .ToDictionary(g => g.Key, g => g.Count()),
                EntityTypeBreakdown = _auditLogs
                    .GroupBy(l => l.EntityType)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            return Ok(stats);
        }
    }
}
