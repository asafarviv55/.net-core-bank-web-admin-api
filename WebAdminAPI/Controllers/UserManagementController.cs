using Microsoft.AspNetCore.Mvc;
using WebAdminAPI.Models;

namespace WebAdminAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserManagementController : ControllerBase
    {
        private static readonly List<SystemUser> _users = new()
        {
            new SystemUser
            {
                UserId = 1,
                Username = "admin",
                Email = "admin@bank.com",
                FullName = "System Administrator",
                Role = "Administrator",
                Department = "IT",
                BranchId = null,
                IsActive = true,
                CreatedDate = new DateTime(2020, 1, 1),
                LastLoginDate = DateTime.Now.AddHours(-2),
                Permissions = new[] { "ALL" }
            },
            new SystemUser
            {
                UserId = 2,
                Username = "manager1",
                Email = "manager1@bank.com",
                FullName = "Branch Manager",
                Role = "Manager",
                Department = "Operations",
                BranchId = 1,
                IsActive = true,
                CreatedDate = new DateTime(2020, 6, 15),
                LastLoginDate = DateTime.Now.AddDays(-1),
                Permissions = new[] { "VIEW_ACCOUNTS", "APPROVE_TRANSACTIONS", "MANAGE_CUSTOMERS" }
            },
            new SystemUser
            {
                UserId = 3,
                Username = "teller1",
                Email = "teller1@bank.com",
                FullName = "Bank Teller",
                Role = "Teller",
                Department = "Customer Service",
                BranchId = 1,
                IsActive = true,
                CreatedDate = new DateTime(2021, 3, 10),
                LastLoginDate = DateTime.Now.AddHours(-4),
                Permissions = new[] { "VIEW_ACCOUNTS", "CREATE_TRANSACTIONS" }
            }
        };

        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(ILogger<UserManagementController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SystemUser>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<SystemUser>> GetAllUsers()
        {
            _logger.LogInformation("Retrieving all users");
            return Ok(_users);
        }

        [HttpGet("{username}")]
        [ProducesResponseType(typeof(SystemUser), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<SystemUser> GetUserByUsername(string username)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound($"User {username} not found");
            }

            return Ok(user);
        }

        [HttpGet("role/{role}")]
        [ProducesResponseType(typeof(IEnumerable<SystemUser>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<SystemUser>> GetUsersByRole(string role)
        {
            var users = _users.Where(u => u.Role.Equals(role, StringComparison.OrdinalIgnoreCase));
            return Ok(users);
        }

        [HttpPost]
        [ProducesResponseType(typeof(SystemUser), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<SystemUser> CreateUser([FromBody] SystemUser user)
        {
            if (_users.Any(u => u.Username == user.Username))
            {
                return BadRequest("Username already exists");
            }

            if (_users.Any(u => u.Email == user.Email))
            {
                return BadRequest("Email already exists");
            }

            user.UserId = _users.Max(u => u.UserId) + 1;
            user.CreatedDate = DateTime.Now;
            user.IsActive = true;
            _users.Add(user);

            _logger.LogInformation($"Created user {user.Username}");
            return CreatedAtAction(nameof(GetUserByUsername), new { username = user.Username }, user);
        }

        [HttpPut("{username}/role")]
        [ProducesResponseType(typeof(SystemUser), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<SystemUser> UpdateUserRole(string username, [FromBody] string newRole)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound($"User {username} not found");
            }

            user.Role = newRole;
            _logger.LogInformation($"Updated role for user {username} to {newRole}");
            return Ok(user);
        }

        [HttpPut("{username}/permissions")]
        [ProducesResponseType(typeof(SystemUser), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<SystemUser> UpdateUserPermissions(string username, [FromBody] string[] permissions)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound($"User {username} not found");
            }

            user.Permissions = permissions;
            _logger.LogInformation($"Updated permissions for user {username}");
            return Ok(user);
        }

        [HttpPut("{username}/deactivate")]
        [ProducesResponseType(typeof(SystemUser), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<SystemUser> DeactivateUser(string username)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound($"User {username} not found");
            }

            user.IsActive = false;
            _logger.LogInformation($"Deactivated user {username}");
            return Ok(user);
        }

        [HttpPost("{username}/login")]
        [ProducesResponseType(typeof(SystemUser), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<SystemUser> RecordLogin(string username)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound($"User {username} not found");
            }

            user.LastLoginDate = DateTime.Now;
            _logger.LogInformation($"Recorded login for user {username}");
            return Ok(user);
        }
    }
}
