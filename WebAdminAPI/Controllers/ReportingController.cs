using Microsoft.AspNetCore.Mvc;
using WebAdminAPI.Models;

namespace WebAdminAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportingController : ControllerBase
    {
        private static readonly List<Report> _reports = new();
        private readonly ILogger<ReportingController> _logger;

        public ReportingController(ILogger<ReportingController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Report>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Report>> GetAllReports()
        {
            _logger.LogInformation("Retrieving all reports");
            return Ok(_reports.OrderByDescending(r => r.GeneratedDate));
        }

        [HttpGet("{reportId}")]
        [ProducesResponseType(typeof(Report), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Report> GetReportById(int reportId)
        {
            var report = _reports.FirstOrDefault(r => r.ReportId == reportId);
            if (report == null)
            {
                return NotFound($"Report {reportId} not found");
            }

            return Ok(report);
        }

        [HttpPost("transaction-summary")]
        [ProducesResponseType(typeof(Report), StatusCodes.Status201Created)]
        public ActionResult<Report> GenerateTransactionSummary([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string generatedBy)
        {
            var report = new Report
            {
                ReportId = _reports.Any() ? _reports.Max(r => r.ReportId) + 1 : 1,
                ReportName = "Transaction Summary Report",
                ReportType = "Transaction Summary",
                GeneratedDate = DateTime.Now,
                GeneratedBy = generatedBy,
                StartDate = startDate,
                EndDate = endDate,
                Status = "Completed",
                Data = new Dictionary<string, object>
                {
                    { "TotalTransactions", 1250 },
                    { "TotalAmount", 2500000m },
                    { "AverageTransactionAmount", 2000m },
                    { "TransactionsByType", new Dictionary<string, int>
                        {
                            { "Transfer", 450 },
                            { "Deposit", 500 },
                            { "Withdrawal", 300 }
                        }
                    }
                }
            };

            _reports.Add(report);
            _logger.LogInformation($"Generated transaction summary report {report.ReportId}");
            return CreatedAtAction(nameof(GetReportById), new { reportId = report.ReportId }, report);
        }

        [HttpPost("account-summary")]
        [ProducesResponseType(typeof(Report), StatusCodes.Status201Created)]
        public ActionResult<Report> GenerateAccountSummary([FromQuery] string generatedBy)
        {
            var report = new Report
            {
                ReportId = _reports.Any() ? _reports.Max(r => r.ReportId) + 1 : 1,
                ReportName = "Account Summary Report",
                ReportType = "Account Summary",
                GeneratedDate = DateTime.Now,
                GeneratedBy = generatedBy,
                StartDate = DateTime.Now.AddMonths(-1),
                EndDate = DateTime.Now,
                Status = "Completed",
                Data = new Dictionary<string, object>
                {
                    { "TotalAccounts", 3500 },
                    { "ActiveAccounts", 3200 },
                    { "InactiveAccounts", 300 },
                    { "TotalBalance", 125000000m },
                    { "AccountsByType", new Dictionary<string, int>
                        {
                            { "Savings", 1500 },
                            { "Checking", 1200 },
                            { "Business", 800 }
                        }
                    }
                }
            };

            _reports.Add(report);
            _logger.LogInformation($"Generated account summary report {report.ReportId}");
            return CreatedAtAction(nameof(GetReportById), new { reportId = report.ReportId }, report);
        }

        [HttpPost("customer-analytics")]
        [ProducesResponseType(typeof(Report), StatusCodes.Status201Created)]
        public ActionResult<Report> GenerateCustomerAnalytics([FromQuery] string generatedBy)
        {
            var report = new Report
            {
                ReportId = _reports.Any() ? _reports.Max(r => r.ReportId) + 1 : 1,
                ReportName = "Customer Analytics Report",
                ReportType = "Customer Analytics",
                GeneratedDate = DateTime.Now,
                GeneratedBy = generatedBy,
                StartDate = DateTime.Now.AddMonths(-3),
                EndDate = DateTime.Now,
                Status = "Completed",
                Data = new Dictionary<string, object>
                {
                    { "TotalCustomers", 3200 },
                    { "NewCustomersThisMonth", 150 },
                    { "CustomersByTier", new Dictionary<string, int>
                        {
                            { "Gold", 800 },
                            { "Silver", 1500 },
                            { "Bronze", 900 }
                        }
                    },
                    { "AverageCustomerBalance", 39062.5m },
                    { "CustomerRetentionRate", 95.5 }
                }
            };

            _reports.Add(report);
            _logger.LogInformation($"Generated customer analytics report {report.ReportId}");
            return CreatedAtAction(nameof(GetReportById), new { reportId = report.ReportId }, report);
        }

        [HttpPost("compliance-report")]
        [ProducesResponseType(typeof(Report), StatusCodes.Status201Created)]
        public ActionResult<Report> GenerateComplianceReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string generatedBy)
        {
            var report = new Report
            {
                ReportId = _reports.Any() ? _reports.Max(r => r.ReportId) + 1 : 1,
                ReportName = "Compliance Report",
                ReportType = "Compliance",
                GeneratedDate = DateTime.Now,
                GeneratedBy = generatedBy,
                StartDate = startDate,
                EndDate = endDate,
                Status = "Completed",
                Data = new Dictionary<string, object>
                {
                    { "TotalReviews", 245 },
                    { "ComplianceRate", 98.5 },
                    { "IssuesFound", 12 },
                    { "IssuesResolved", 10 },
                    { "PendingIssues", 2 },
                    { "HighRiskAlerts", 3 }
                }
            };

            _reports.Add(report);
            _logger.LogInformation($"Generated compliance report {report.ReportId}");
            return CreatedAtAction(nameof(GetReportById), new { reportId = report.ReportId }, report);
        }

        [HttpDelete("{reportId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteReport(int reportId)
        {
            var report = _reports.FirstOrDefault(r => r.ReportId == reportId);
            if (report == null)
            {
                return NotFound($"Report {reportId} not found");
            }

            _reports.Remove(report);
            _logger.LogInformation($"Deleted report {reportId}");
            return NoContent();
        }
    }
}
