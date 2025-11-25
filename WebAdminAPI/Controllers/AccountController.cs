using Microsoft.AspNetCore.Mvc;
using WebAdminAPI.Models;

namespace WebAdminAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private static readonly List<Account> _accounts = new()
        {
            new Account
            {
                AccountId = 1,
                AccountNumber = "ACC1001",
                AccountType = "Savings",
                CustomerName = "John Doe",
                Balance = 25000.00m,
                Currency = "USD",
                OpenedDate = new DateTime(2020, 5, 15),
                Status = "Active",
                BranchId = 1,
                IsActive = true
            },
            new Account
            {
                AccountId = 2,
                AccountNumber = "ACC1002",
                AccountType = "Checking",
                CustomerName = "Jane Smith",
                Balance = 8500.50m,
                Currency = "USD",
                OpenedDate = new DateTime(2021, 8, 22),
                Status = "Active",
                BranchId = 2,
                IsActive = true
            },
            new Account
            {
                AccountId = 3,
                AccountNumber = "ACC1003",
                AccountType = "Business",
                CustomerName = "ABC Corporation",
                Balance = 150000.00m,
                Currency = "USD",
                OpenedDate = new DateTime(2019, 3, 10),
                Status = "Active",
                BranchId = 1,
                IsActive = true
            }
        };

        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Account>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Account>> GetAllAccounts()
        {
            _logger.LogInformation("Retrieving all accounts");
            return Ok(_accounts);
        }

        [HttpGet("{accountNumber}")]
        [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Account> GetAccountByNumber(string accountNumber)
        {
            var account = _accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            if (account == null)
            {
                _logger.LogWarning($"Account {accountNumber} not found");
                return NotFound($"Account {accountNumber} not found");
            }

            return Ok(account);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Account), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Account> CreateAccount([FromBody] Account account)
        {
            if (_accounts.Any(a => a.AccountNumber == account.AccountNumber))
            {
                return BadRequest("Account number already exists");
            }

            account.AccountId = _accounts.Max(a => a.AccountId) + 1;
            account.OpenedDate = DateTime.Now;
            _accounts.Add(account);

            _logger.LogInformation($"Created account {account.AccountNumber}");
            return CreatedAtAction(nameof(GetAccountByNumber), new { accountNumber = account.AccountNumber }, account);
        }

        [HttpPut("{accountNumber}/balance")]
        [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Account> UpdateBalance(string accountNumber, [FromBody] decimal newBalance)
        {
            var account = _accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            if (account == null)
            {
                return NotFound($"Account {accountNumber} not found");
            }

            account.Balance = newBalance;
            _logger.LogInformation($"Updated balance for account {accountNumber} to {newBalance}");
            return Ok(account);
        }

        [HttpPut("{accountNumber}/status")]
        [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Account> UpdateStatus(string accountNumber, [FromBody] string status)
        {
            var account = _accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            if (account == null)
            {
                return NotFound($"Account {accountNumber} not found");
            }

            account.Status = status;
            account.IsActive = status.Equals("Active", StringComparison.OrdinalIgnoreCase);
            _logger.LogInformation($"Updated status for account {accountNumber} to {status}");
            return Ok(account);
        }

        [HttpDelete("{accountNumber}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteAccount(string accountNumber)
        {
            var account = _accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            if (account == null)
            {
                return NotFound($"Account {accountNumber} not found");
            }

            _accounts.Remove(account);
            _logger.LogInformation($"Deleted account {accountNumber}");
            return NoContent();
        }
    }
}
