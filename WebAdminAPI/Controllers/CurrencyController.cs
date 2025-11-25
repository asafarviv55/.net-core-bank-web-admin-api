using Microsoft.AspNetCore.Mvc;

namespace WebAdminAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : ControllerBase
    {
        private static readonly Random _random = new Random();
        private readonly ILogger<CurrencyController> _logger;

        private static readonly string[] CurrencyNames = new[]
        {
            "USD", "EUR", "GBP", "JPY", "CHF", "CAD", "AUD", "CNY", "INR", "BRL"
        };

        private static readonly Dictionary<string, double> BaseRates = new()
        {
            { "USD", 1.00 },
            { "EUR", 0.92 },
            { "GBP", 0.79 },
            { "JPY", 149.50 },
            { "CHF", 0.88 },
            { "CAD", 1.36 },
            { "AUD", 1.53 },
            { "CNY", 7.24 },
            { "INR", 83.12 },
            { "BRL", 4.97 }
        };

        public CurrencyController(ILogger<CurrencyController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetCurrenciesRate")]
        [ProducesResponseType(typeof(IEnumerable<Currency>), StatusCodes.Status200OK)]
        public IEnumerable<Currency> Get()
        {
            return CurrencyNames.Select(name => new Currency
            {
                Name = name,
                Rate = Math.Round(BaseRates[name] * (0.98 + _random.NextDouble() * 0.04), 4)
            });
        }

        [HttpGet("{name}", Name = "GetCurrencyByName")]
        [ProducesResponseType(typeof(Currency), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Currency> GetByName(string name)
        {
            var currencyName = name.ToUpperInvariant();
            if (!BaseRates.ContainsKey(currencyName))
            {
                return NotFound($"Currency '{name}' not found");
            }

            return new Currency
            {
                Name = currencyName,
                Rate = Math.Round(BaseRates[currencyName] * (0.98 + _random.NextDouble() * 0.04), 4)
            };
        }
    }
}