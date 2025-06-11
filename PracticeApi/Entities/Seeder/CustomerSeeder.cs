using Bogus;
using PracticeApi.Data;
using PracticeApi.Entities.Model;

namespace PracticeApi.Entities.Seeder
{
    public class CustomerSeeder
    {
        public static async Task SeedAsync(AppDbContext context, int count = 100000)
        {
            if(context.Customers.Any()) return;

            var dummyCustomer = new Faker<Customer>()
                .RuleFor(c => c.Name, f => f.Name.FullName())
                .RuleFor(c => c.Email, f => f.Internet.Email())
                .RuleFor(c => c.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(c => c.Address, f => f.Address.StreetAddress())
                .RuleFor(c => c.City, f => f.Address.City())
                .RuleFor(c => c.Country, f => f.Address.Country());

            var customers = dummyCustomer.Generate(count);
            await context.Customers.AddRangeAsync(customers);
            await context.SaveChangesAsync();
        }
    }
}
