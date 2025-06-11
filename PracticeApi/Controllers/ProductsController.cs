using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeApi.Data;
using PracticeApi.Entities.Model;
using PracticeApi.Services;

namespace PracticeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]

    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(AppDbContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [HttpPost]
        [ActionName("Create")]
        public async Task<IActionResult> Create(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }


        [HttpPut("{id}")]
        [ActionName("Update")]
        public async Task<IActionResult> Update(int id, Product product)
        {
            if (id != product.Id) return BadRequest();
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("{id}")]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpGet("{id}")]
        [ActionName("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return product == null ? NotFound() : Ok(product);
        }


        [HttpGet]
        [ActionName("GetAll")]
        public async Task<IActionResult> Get() => Ok(await _context.Products.ToListAsync());


        [HttpGet]
        [ActionName("SearchEf")]
        public async Task<IActionResult> SearchEf(string query)
        {
            var result = await _context.Products
                .Where(p => p.Name.Contains(query) ||
                            p.Description.Contains(query) ||
                            p.Category.Contains(query))
                .ToListAsync();
            return Ok(result);
        }


        [HttpGet]
        [ActionName("SearchEs")]
        public async Task<IActionResult> SearchEs([FromServices] ElasticService elasticService, string query)
        {
            //var result = await elasticService.SearchAsync(query);
            //return Ok(result);

            var result = await elasticService.SearchAsync<Product>(query, f => f
                                .Field(p => p.Name)
                                .Field(p => p.Description)
                                .Field(p => p.Category));

            return Ok(result);
        }
    }
}
