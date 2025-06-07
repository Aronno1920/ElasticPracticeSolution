using Bogus;
using SalesApi.Data;
using SalesCore.Entities;

namespace SalesApi.Entities
{
    public class ProductSeeder
    {
        public static async Task SeedAsync(AppDbContext context, int count = 10000)
        {
            if (context.Products.Any()) return;

            var faker = new Faker<Product>()
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.Price, f => f.Random.Decimal(10, 1000))
                .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0]);

            var products = faker.Generate(count);
            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}
