using Nest;
using PracticeApi.Entities.Model;

namespace PracticeApi.Services
{
    public class ElasticService
    {
        private readonly IElasticClient _client;

        public ElasticService(IElasticClient client)
        {
            _client = client;
        }

        private string GetIndexName<T>() where T : class
        {
            return typeof(T).Name.ToLowerInvariant() + "s"; // e.g., "products", "customers"
        }

        public async Task IndexAsync<T>(T document) where T : class
        {
            var response = await _client.IndexAsync(document, i => i.Index(GetIndexName<T>()));
            if (!response.IsValid)
            {
                Console.WriteLine($"Index failed: {response.ServerError?.Error.Reason}");
            }
        }

        public async Task BulkIndexAsync<T>(IEnumerable<T> documents) where T : class
        {
            var indexName = GetIndexName<T>();
            var response = await _client.BulkAsync(b => b
                .Index(indexName)
                .IndexMany(documents)
            );

            if (response.Errors)
            {
                foreach (var item in response.ItemsWithErrors)
                {
                    Console.WriteLine($"Failed to index doc {item.Id}: {item.Error.Reason}");
                }
            }
        }

        public async Task<IEnumerable<T>> SearchAsync<T>(string query, Func<FieldsDescriptor<T>, IPromise<Fields>> fieldSelector) where T : class
        {
            var response = await _client.SearchAsync<T>(s => s
                .Index(GetIndexName<T>())
                .Query(q => q
                    .MultiMatch(m => m
                        .Fields(fieldSelector)
                        .Query(query)
                    )
                )
            );

            return response.Documents;
        }
    }


    //public class ElasticService
    //{
    //    private readonly IElasticClient _client;

    //    public ElasticService(IElasticClient client)
    //    {
    //        _client = client;
    //    }

    //    public async Task IndexAsync(Product product)
    //    {
    //        await _client.IndexDocumentAsync(product);
    //    }

    //    public async Task BulkIndexAsync(IEnumerable<Product> products)
    //    {
    //        await _client.IndexManyAsync(products);
    //    }

    //    public async Task<IEnumerable<Product>> SearchAsync(string query)
    //    {
    //        var response = await _client.SearchAsync<Product>(s => s
    //            .Query(q => q
    //                .MultiMatch(m => m
    //                    .Fields(f => f
    //                        .Field(p => p.Name)
    //                        .Field(p => p.Description)
    //                        .Field(p => p.Category)
    //                    )
    //                    .Query(query)
    //                )
    //            )
    //        );

    //        return response.Documents;
    //    }
    //}
}
