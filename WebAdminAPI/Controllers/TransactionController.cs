using Microsoft.AspNetCore.Mvc;
using WebAdminAPI.Models;

namespace WebAdminAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private static readonly List<Transaction> _transactions = new()
        {
            new Transaction
            {
                TransactionId = 1,
                TransactionNumber = "TXN001",
                TransactionDate = DateTime.Now.AddDays(-2),
                FromAccount = "ACC1001",
                ToAccount = "ACC1002",
                Amount = 1500.00m,
                TransactionType = "Transfer",
                Description = "Payment for services",
                Status = "Completed",
                InitiatedBy = "John Doe",
                ApprovedBy = "Admin1"
            },
            new Transaction
            {
                TransactionId = 2,
                TransactionNumber = "TXN002",
                TransactionDate = DateTime.Now.AddDays(-1),
                FromAccount = "ACC1003",
                ToAccount = "",
                Amount = 5000.00m,
                TransactionType = "Withdrawal",
                Description = "Cash withdrawal",
                Status = "Completed",
                InitiatedBy = "ABC Corporation",
                ApprovedBy = "Admin2"
            }
        };

        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ILogger<TransactionController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Transaction>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Transaction>> GetAllTransactions()
        {
            _logger.LogInformation("Retrieving all transactions");
            return Ok(_transactions.OrderByDescending(t => t.TransactionDate));
        }

        [HttpGet("{transactionNumber}")]
        [ProducesResponseType(typeof(Transaction), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Transaction> GetTransactionByNumber(string transactionNumber)
        {
            var transaction = _transactions.FirstOrDefault(t => t.TransactionNumber == transactionNumber);
            if (transaction == null)
            {
                return NotFound($"Transaction {transactionNumber} not found");
            }

            return Ok(transaction);
        }

        [HttpGet("account/{accountNumber}")]
        [ProducesResponseType(typeof(IEnumerable<Transaction>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Transaction>> GetTransactionsByAccount(string accountNumber)
        {
            var transactions = _transactions
                .Where(t => t.FromAccount == accountNumber || t.ToAccount == accountNumber)
                .OrderByDescending(t => t.TransactionDate);

            return Ok(transactions);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Transaction), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Transaction> CreateTransaction([FromBody] Transaction transaction)
        {
            if (transaction.Amount <= 0)
            {
                return BadRequest("Transaction amount must be greater than zero");
            }

            transaction.TransactionId = _transactions.Any() ? _transactions.Max(t => t.TransactionId) + 1 : 1;
            transaction.TransactionNumber = $"TXN{transaction.TransactionId:D6}";
            transaction.TransactionDate = DateTime.Now;
            transaction.Status = "Pending";
            _transactions.Add(transaction);

            _logger.LogInformation($"Created transaction {transaction.TransactionNumber}");
            return CreatedAtAction(nameof(GetTransactionByNumber),
                new { transactionNumber = transaction.TransactionNumber }, transaction);
        }

        [HttpPut("{transactionNumber}/approve")]
        [ProducesResponseType(typeof(Transaction), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Transaction> ApproveTransaction(string transactionNumber, [FromBody] string approvedBy)
        {
            var transaction = _transactions.FirstOrDefault(t => t.TransactionNumber == transactionNumber);
            if (transaction == null)
            {
                return NotFound($"Transaction {transactionNumber} not found");
            }

            transaction.Status = "Completed";
            transaction.ApprovedBy = approvedBy;
            _logger.LogInformation($"Approved transaction {transactionNumber} by {approvedBy}");
            return Ok(transaction);
        }

        [HttpPut("{transactionNumber}/reject")]
        [ProducesResponseType(typeof(Transaction), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Transaction> RejectTransaction(string transactionNumber, [FromBody] string reason)
        {
            var transaction = _transactions.FirstOrDefault(t => t.TransactionNumber == transactionNumber);
            if (transaction == null)
            {
                return NotFound($"Transaction {transactionNumber} not found");
            }

            transaction.Status = "Rejected";
            transaction.Description += $" | Rejection Reason: {reason}";
            _logger.LogInformation($"Rejected transaction {transactionNumber}");
            return Ok(transaction);
        }
    }
}
