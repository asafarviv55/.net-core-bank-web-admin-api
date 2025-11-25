using Microsoft.AspNetCore.Mvc;
using WebAdminAPI.Models;

namespace WebAdminAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private static readonly List<Customer> _customers = new()
        {
            new Customer
            {
                CustomerId = 1,
                CustomerNumber = "CUST001",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@email.com",
                PhoneNumber = "555-0101",
                DateOfBirth = new DateTime(1985, 5, 15),
                Address = "123 Main St",
                City = "New York",
                State = "NY",
                ZipCode = "10001",
                CustomerTier = "Gold",
                RegistrationDate = new DateTime(2020, 1, 10),
                IsActive = true,
                KYCStatus = "Verified"
            },
            new Customer
            {
                CustomerId = 2,
                CustomerNumber = "CUST002",
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@email.com",
                PhoneNumber = "555-0102",
                DateOfBirth = new DateTime(1990, 8, 22),
                Address = "456 Oak Ave",
                City = "Los Angeles",
                State = "CA",
                ZipCode = "90001",
                CustomerTier = "Silver",
                RegistrationDate = new DateTime(2021, 3, 15),
                IsActive = true,
                KYCStatus = "Verified"
            }
        };

        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ILogger<CustomerController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Customer>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Customer>> GetAllCustomers()
        {
            _logger.LogInformation("Retrieving all customers");
            return Ok(_customers);
        }

        [HttpGet("{customerNumber}")]
        [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Customer> GetCustomerByNumber(string customerNumber)
        {
            var customer = _customers.FirstOrDefault(c => c.CustomerNumber == customerNumber);
            if (customer == null)
            {
                return NotFound($"Customer {customerNumber} not found");
            }

            return Ok(customer);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<Customer>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Customer>> SearchCustomers([FromQuery] string? email, [FromQuery] string? phone)
        {
            var results = _customers.AsQueryable();

            if (!string.IsNullOrEmpty(email))
            {
                results = results.Where(c => c.Email.Contains(email, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(phone))
            {
                results = results.Where(c => c.PhoneNumber.Contains(phone));
            }

            return Ok(results.ToList());
        }

        [HttpPost]
        [ProducesResponseType(typeof(Customer), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Customer> CreateCustomer([FromBody] Customer customer)
        {
            if (_customers.Any(c => c.Email == customer.Email))
            {
                return BadRequest("Customer with this email already exists");
            }

            customer.CustomerId = _customers.Max(c => c.CustomerId) + 1;
            customer.CustomerNumber = $"CUST{customer.CustomerId:D6}";
            customer.RegistrationDate = DateTime.Now;
            customer.IsActive = true;
            _customers.Add(customer);

            _logger.LogInformation($"Created customer {customer.CustomerNumber}");
            return CreatedAtAction(nameof(GetCustomerByNumber),
                new { customerNumber = customer.CustomerNumber }, customer);
        }

        [HttpPut("{customerNumber}")]
        [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Customer> UpdateCustomer(string customerNumber, [FromBody] Customer updatedCustomer)
        {
            var customer = _customers.FirstOrDefault(c => c.CustomerNumber == customerNumber);
            if (customer == null)
            {
                return NotFound($"Customer {customerNumber} not found");
            }

            customer.FirstName = updatedCustomer.FirstName;
            customer.LastName = updatedCustomer.LastName;
            customer.Email = updatedCustomer.Email;
            customer.PhoneNumber = updatedCustomer.PhoneNumber;
            customer.Address = updatedCustomer.Address;
            customer.City = updatedCustomer.City;
            customer.State = updatedCustomer.State;
            customer.ZipCode = updatedCustomer.ZipCode;
            customer.CustomerTier = updatedCustomer.CustomerTier;

            _logger.LogInformation($"Updated customer {customerNumber}");
            return Ok(customer);
        }

        [HttpPut("{customerNumber}/kyc")]
        [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Customer> UpdateKYCStatus(string customerNumber, [FromBody] string kycStatus)
        {
            var customer = _customers.FirstOrDefault(c => c.CustomerNumber == customerNumber);
            if (customer == null)
            {
                return NotFound($"Customer {customerNumber} not found");
            }

            customer.KYCStatus = kycStatus;
            _logger.LogInformation($"Updated KYC status for customer {customerNumber} to {kycStatus}");
            return Ok(customer);
        }

        [HttpDelete("{customerNumber}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeactivateCustomer(string customerNumber)
        {
            var customer = _customers.FirstOrDefault(c => c.CustomerNumber == customerNumber);
            if (customer == null)
            {
                return NotFound($"Customer {customerNumber} not found");
            }

            customer.IsActive = false;
            _logger.LogInformation($"Deactivated customer {customerNumber}");
            return NoContent();
        }
    }
}
