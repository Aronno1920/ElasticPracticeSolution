using Microsoft.EntityFrameworkCore;
using PracticeApi.Data;

namespace PracticeApi.Services
{
    public class ElasticSyncService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ElasticSyncService> _logger;

        public ElasticSyncService(IServiceProvider serviceProvider, ILogger<ElasticSyncService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var elasticService = scope.ServiceProvider.GetRequiredService<ElasticService>();

            try
            {
                // Sync Products
                var products = await db.Products.ToListAsync(stoppingToken);
                if (products.Any())
                {
                    await elasticService.BulkIndexAsync(products);
                    _logger.LogInformation($"Indexed {products.Count} products to Elasticsearch.");
                }

                // Sync Customers
                var customers = await db.Customers.ToListAsync(stoppingToken);
                if (customers.Any())
                {
                    await elasticService.BulkIndexAsync(customers);
                    _logger.LogInformation($"Indexed {customers.Count} customers to Elasticsearch.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Elasticsearch sync");
            }
        }


        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    using var scope = _serviceProvider.CreateScope();
        //    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        //    var es = scope.ServiceProvider.GetRequiredService<ElasticService>();

        //    var products = await db.Products.ToListAsync(stoppingToken);
        //    await es.BulkIndexAsync(products);
        //}
    }
}
