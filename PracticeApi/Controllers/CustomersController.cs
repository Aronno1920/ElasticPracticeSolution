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


        [HttpDelete]
        [ActionName("DeleteAll")]
        public async Task<IActionResult> DeleteAll([FromServices] ElasticService elasticService)
        {
            var customers = await _context.Customers.ToListAsync();

            if (customers.Count == 0)
                return NoContent();

            _context.Customers.RemoveRange(customers);
            await _context.SaveChangesAsync();

            var ids = customers.Select(p => p.Id.ToString());
            await elasticService.BulkDeleteAsync<Product>(ids);

            return Ok(new { message = $"{customers.Count} customers deleted from DB and Elasticsearch." });
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


        [HttpGet]
        [ActionName("CountCustomerEs")]
        public async Task<IActionResult> CountCustomers([FromServices] ElasticService elasticService)
        {
            long total = await elasticService.CountAsync<Customer>();
            return Ok(new { TotalCustomersInElastic = total });
        }
    }
}
