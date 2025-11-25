namespace WebAdminAPI.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime OpenedDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public bool IsActive { get; set; }
    }
}
