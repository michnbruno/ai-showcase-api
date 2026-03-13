using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;

namespace AiShowcase.Api.Services
{
    public class SearchService
    {
        private readonly SearchIndexClient _indexClient;
        private readonly SearchClient _searchClient;
        private const string IndexName = "documents";

        public SearchService(string endpoint, string apiKey)
        {
            var credential = new AzureKeyCredential(apiKey);
            _indexClient = new SearchIndexClient(new Uri(endpoint), credential);
            _searchClient = new SearchClient(new Uri(endpoint), IndexName, credential);
        }

        public async Task EnsureIndexExistsAsync()
        {
            var fields = new FieldBuilder().Build(typeof(DocumentIndexEntry));
            var index = new SearchIndex(IndexName, fields);
            await _indexClient.CreateOrUpdateIndexAsync(index);
        }

        public async Task IndexDocumentAsync(string id, string fileName, string content)
        {
            var doc = new DocumentIndexEntry
            {
                Id = id,
                FileName = fileName,
                Content = content,
                IndexedAt = DateTimeOffset.UtcNow
            };

            await _searchClient.MergeOrUploadDocumentsAsync(
                new[] { doc });
        }

        public async Task<List<DocumentSearchResult>> SearchAsync(string query)
        {
            var options = new SearchOptions
            {
                Size = 5,
                IncludeTotalCount = true
            };

            var response = await _searchClient.SearchAsync<DocumentIndexEntry>(
                query, options);

            var results = new List<DocumentSearchResult>();
            await foreach (var result in response.Value.GetResultsAsync())
            {
                results.Add(new DocumentSearchResult
                {
                    Id = result.Document.Id,
                    FileName = result.Document.FileName,
                    Score = result.Score ?? 0,
                    Snippet = result.Document.Content?.Length > 200
                        ? result.Document.Content[..200] + "..."
                        : result.Document.Content ?? ""
                });
            }

            return results;
        }
    }

    public class DocumentIndexEntry
    {
        [SimpleField(IsKey = true, IsFilterable = true)]
        public string Id { get; set; } = string.Empty;

        [SearchableField(IsFilterable = true, IsSortable = true)]
        public string FileName { get; set; } = string.Empty;

        [SearchableField]
        public string Content { get; set; } = string.Empty;

        [SimpleField(IsSortable = true, IsFilterable = true)]
        public DateTimeOffset IndexedAt { get; set; }
    }

    public class DocumentSearchResult
    {
        public string Id { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public double Score { get; set; }
        public string Snippet { get; set; } = string.Empty;
    }
}