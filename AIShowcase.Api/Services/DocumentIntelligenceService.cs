using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace AiShowcase.Api.Services
{
    public class DocumentIntelligenceService
    {
        private readonly DocumentAnalysisClient _client;

        public DocumentIntelligenceService(string endpoint, string apiKey)
        {
            _client = new DocumentAnalysisClient(
                new Uri(endpoint),
                new AzureKeyCredential(apiKey)
            );
        }

        public async Task<string> AnalyzeDocumentAsync(Stream documentStream)
        {
            var operation = await _client.AnalyzeDocumentAsync(
                WaitUntil.Completed,
                "prebuilt-read",
                documentStream
            );

            var result = operation.Value;
            var extractedText = string.Join("\n", result.Pages
                .SelectMany(p => p.Lines)
                .Select(l => l.Content));

            return extractedText;
        }
    }
}