using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeApi.Data;
using PracticeApi.Entities.Model;
using PracticeApi.Services;

namespace PracticeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]

    public class CustomersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(AppDbContext context, ILogger<CustomersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [ActionName("SearchEf")]
        public async Task<IActionResult> SearchEf(string query)
        {
            var result = await _context.Customers
                .Where(p => p.Name.Contains(query) ||
                            p.Email.Contains(query) ||
                            p.PhoneNumber.Contains(query) ||
                            p.City.Contains(query) ||
                            p.Country.Contains(query))
                .ToListAsync();

            _logger.LogInformation($"Total {result.Count} customers found from EntityFrameWork.");

            return Ok(result);
        }


        [HttpGet]
        [ActionName("SearchEs")]
        public async Task<IActionResult> SearchEs([FromServices] ElasticService elasticService, string query)
        {
            var result = await elasticService.SearchAsync<Customer>(query, f => f
                        .Field(c => c.Name)
                        .Field(c => c.Email)
                        .Field(c => c.PhoneNumber)
                        .Field(c => c.City)
                        .Field(c => c.Country));

            _logger.LogInformation($"Customer found to Elasticsearch.");

            return Ok(result);
        }
    }
}
