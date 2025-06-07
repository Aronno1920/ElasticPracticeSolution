using Nest;
using SalesCore.Entities;

namespace SalesApi.Services
{
    public class ElasticService
    {
        private readonly IElasticClient _client;

        public ElasticService(IElasticClient client)
        {
            _client = client;
        }

        public async Task IndexAsync(Product product)
        {
            await _client.IndexDocumentAsync(product);
        }

        public async Task BulkIndexAsync(IEnumerable<Product> products)
        {
            await _client.IndexManyAsync(products);
        }

        public async Task<IEnumerable<Product>> SearchAsync(string query)
        {
            var response = await _client.SearchAsync<Product>(s => s
                .Query(q => q
                    .MultiMatch(m => m
                        .Fields(f => f
                            .Field(p => p.Name)
                            .Field(p => p.Description)
                            .Field(p => p.Category)
                        )
                        .Query(query)
                    )
                )
            );

            return response.Documents;
        }
    }
}
