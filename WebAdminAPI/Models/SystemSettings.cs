namespace WebAdminAPI.Models
{
    public class SystemSettings
    {
        public int SettingId { get; set; }
        public string SettingKey { get; set; } = string.Empty;
        public string SettingValue { get; set; } = string.Empty;
        public string SettingCategory { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
