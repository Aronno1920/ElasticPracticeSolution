using Microsoft.EntityFrameworkCore;
using PracticeApi.Data;

namespace PracticeApi.Services
{
    public class ElasticSyncService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ElasticSyncService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var es = scope.ServiceProvider.GetRequiredService<ElasticService>();

            var products = await db.Products.ToListAsync(stoppingToken);
            await es.BulkIndexAsync(products);
        }
    }

}
