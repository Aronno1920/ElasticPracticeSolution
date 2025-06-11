﻿using Bogus;
using PracticeApi.Data;
using PracticeApi.Entities.Model;

namespace PracticeApi.Entities.Seeder
{
    public class ProductSeeder
    {
        public static async Task SeedAsync(AppDbContext context, int count = 200000)
        {
            if (context.Products.Any()) return;

            var faker = new Faker<Product>()
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.Price, f => f.Random.Decimal(10, 10000))
                .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0]);

            var products = faker.Generate(count);
            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}
