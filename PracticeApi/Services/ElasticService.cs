using Nest;

namespace PracticeApi.Services
{
    public class ElasticService
    {
        private readonly IElasticClient _client;

        public ElasticService(IElasticClient client)
        {
            _client = client;
        }

        #region Add or Update single/bulk documents
        // Add or Update single document
        public async Task IndexAsync<T>(T document, string id = null) where T : class
        {
            var response = await _client.IndexAsync(document, i => i
                .Index(GetIndexName<T>())
                .Id(id)
                .Refresh(Elasticsearch.Net.Refresh.WaitFor)
            );

            if (!response.IsValid)
            {
                Console.WriteLine($"Indexing failed: {response.ServerError?.Error.Reason}");
            }
        }

        // Bulk add or update
        public async Task BulkIndexAsync<T>(IEnumerable<T> documents) where T : class
        {
            var indexName = GetIndexName<T>();
            var response = await _client.BulkAsync(b => b
                .Index(indexName)
                .IndexMany(documents)
                .Refresh(Elasticsearch.Net.Refresh.WaitFor)
            );

            if (response.Errors)
            {
                foreach (var item in response.ItemsWithErrors)
                {
                    Console.WriteLine($"Failed to index document {item.Id}: {item.Error.Reason}");
                }
            }
        }
        #endregion

        #region Delete single/bulk documents
        // Delete single document by Id
        public async Task DeleteAsync<T>(string id) where T : class
        {
            var response = await _client.DeleteAsync<T>(id, d => d
                .Index(GetIndexName<T>())
                .Refresh(Elasticsearch.Net.Refresh.WaitFor)
            );

            if (!response.IsValid)
            {
                Console.WriteLine($"Delete failed: {response.ServerError?.Error.Reason}");
            }
        }

        // Bulk delete documents by Ids
        public async Task BulkDeleteAsync<T>(IEnumerable<string> ids) where T : class
        {
            var indexName = GetIndexName<T>();
            var response = await _client.BulkAsync(b =>
            {
                foreach (var id in ids)
                {
                    b.Delete<T>(d => d.Index(indexName).Id(id));
                }
                return b.Refresh(Elasticsearch.Net.Refresh.WaitFor);
            });

            if (response.Errors)
            {
                foreach (var item in response.ItemsWithErrors)
                {
                    Console.WriteLine($"Failed to delete document {item.Id}: {item.Error.Reason}");
                }
            }
        }
        #endregion

        #region Search documents
        private string GetIndexName<T>() where T : class
        {
            return typeof(T).Name.ToLowerInvariant() + "s"; // e.g. products, customers
        }

        // Count total item query
        public async Task<long> CountAsync<T>() where T : class
        {
            var response = await _client.CountAsync<T>(c => c
                .Index(GetIndexName<T>())
            );

            if (!response.IsValid)
            {
                Console.WriteLine($"Count failed: {response.ServerError?.Error.Reason}");
                return 0;
            }

            return response.Count;
        }

        // Search with MultiMatch query
        public async Task<ISearchResponse<T>> SearchAsync<T>(string query, Func<FieldsDescriptor<T>, IPromise<Fields>> fieldSelector) where T : class
        {
            var response = await _client.SearchAsync<T>(s => s
                .Index(GetIndexName<T>())
                .Size(10000)
                .Query(q => q
                    .MultiMatch(m => m
                        .Fields(fieldSelector)
                        .Type(TextQueryType.PhrasePrefix)
                        .Query(query)
                    )
                )
            );

            return response;
        }
        #endregion
    }
}

#region For Single Entity Example

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

#endregion