using Microsoft.AspNetCore.Mvc;
using WebAdminAPI.Models;

namespace WebAdminAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemSettingsController : ControllerBase
    {
        private static readonly List<SystemSettings> _settings = new()
        {
            new SystemSettings
            {
                SettingId = 1,
                SettingKey = "MAX_DAILY_WITHDRAWAL_LIMIT",
                SettingValue = "50000",
                SettingCategory = "Transaction Limits",
                Description = "Maximum daily withdrawal limit per account",
                LastModified = DateTime.Now.AddMonths(-1),
                ModifiedBy = "admin",
                IsActive = true
            },
            new SystemSettings
            {
                SettingId = 2,
                SettingKey = "MIN_ACCOUNT_BALANCE",
                SettingValue = "1000",
                SettingCategory = "Account Rules",
                Description = "Minimum balance required for savings accounts",
                LastModified = DateTime.Now.AddMonths(-2),
                ModifiedBy = "admin",
                IsActive = true
            },
            new SystemSettings
            {
                SettingId = 3,
                SettingKey = "TRANSACTION_FEE_PERCENTAGE",
                SettingValue = "0.5",
                SettingCategory = "Fees",
                Description = "Percentage fee for wire transfers",
                LastModified = DateTime.Now.AddWeeks(-1),
                ModifiedBy = "admin",
                IsActive = true
            },
            new SystemSettings
            {
                SettingId = 4,
                SettingKey = "SESSION_TIMEOUT_MINUTES",
                SettingValue = "30",
                SettingCategory = "Security",
                Description = "User session timeout in minutes",
                LastModified = DateTime.Now.AddDays(-10),
                ModifiedBy = "admin",
                IsActive = true
            },
            new SystemSettings
            {
                SettingId = 5,
                SettingKey = "ENABLE_TWO_FACTOR_AUTH",
                SettingValue = "true",
                SettingCategory = "Security",
                Description = "Enable two-factor authentication for all users",
                LastModified = DateTime.Now.AddDays(-5),
                ModifiedBy = "admin",
                IsActive = true
            }
        };

        private readonly ILogger<SystemSettingsController> _logger;

        public SystemSettingsController(ILogger<SystemSettingsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SystemSettings>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<SystemSettings>> GetAllSettings()
        {
            _logger.LogInformation("Retrieving all system settings");
            return Ok(_settings);
        }

        [HttpGet("{settingKey}")]
        [ProducesResponseType(typeof(SystemSettings), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<SystemSettings> GetSettingByKey(string settingKey)
        {
            var setting = _settings.FirstOrDefault(s => s.SettingKey.Equals(settingKey, StringComparison.OrdinalIgnoreCase));
            if (setting == null)
            {
                return NotFound($"Setting {settingKey} not found");
            }

            return Ok(setting);
        }

        [HttpGet("category/{category}")]
        [ProducesResponseType(typeof(IEnumerable<SystemSettings>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<SystemSettings>> GetSettingsByCategory(string category)
        {
            var settings = _settings.Where(s => s.SettingCategory.Equals(category, StringComparison.OrdinalIgnoreCase));
            return Ok(settings);
        }

        [HttpPost]
        [ProducesResponseType(typeof(SystemSettings), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<SystemSettings> CreateSetting([FromBody] SystemSettings setting)
        {
            if (_settings.Any(s => s.SettingKey.Equals(setting.SettingKey, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("Setting with this key already exists");
            }

            setting.SettingId = _settings.Max(s => s.SettingId) + 1;
            setting.LastModified = DateTime.Now;
            setting.IsActive = true;
            _settings.Add(setting);

            _logger.LogInformation($"Created setting {setting.SettingKey}");
            return CreatedAtAction(nameof(GetSettingByKey), new { settingKey = setting.SettingKey }, setting);
        }

        [HttpPut("{settingKey}")]
        [ProducesResponseType(typeof(SystemSettings), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<SystemSettings> UpdateSetting(string settingKey, [FromBody] SystemSettings updatedSetting)
        {
            var setting = _settings.FirstOrDefault(s => s.SettingKey.Equals(settingKey, StringComparison.OrdinalIgnoreCase));
            if (setting == null)
            {
                return NotFound($"Setting {settingKey} not found");
            }

            setting.SettingValue = updatedSetting.SettingValue;
            setting.Description = updatedSetting.Description;
            setting.LastModified = DateTime.Now;
            setting.ModifiedBy = updatedSetting.ModifiedBy;

            _logger.LogInformation($"Updated setting {settingKey} to value {updatedSetting.SettingValue}");
            return Ok(setting);
        }

        [HttpDelete("{settingKey}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteSetting(string settingKey)
        {
            var setting = _settings.FirstOrDefault(s => s.SettingKey.Equals(settingKey, StringComparison.OrdinalIgnoreCase));
            if (setting == null)
            {
                return NotFound($"Setting {settingKey} not found");
            }

            setting.IsActive = false;
            _logger.LogInformation($"Deactivated setting {settingKey}");
            return NoContent();
        }
    }
}
