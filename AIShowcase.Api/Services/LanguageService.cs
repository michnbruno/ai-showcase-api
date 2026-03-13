using Azure;
using Azure.AI.TextAnalytics;

namespace AiShowcase.Api.Services
{
    public class LanguageService
    {
        private readonly TextAnalyticsClient _client;

        public LanguageService(string endpoint, string apiKey)
        {
            _client = new TextAnalyticsClient(
                new Uri(endpoint),
                new AzureKeyCredential(apiKey)
            );
        }

        public async Task<LanguageAnalysisResult> AnalyzeTextAsync(string text)
        {
            var tasks = new TextAnalyticsActions
            {
                ExtractKeyPhrasesActions = new[] { new ExtractKeyPhrasesAction() },
                RecognizeEntitiesActions = new[] { new RecognizeEntitiesAction() },
                AnalyzeSentimentActions = new[] { new AnalyzeSentimentAction() }
            };

            var operation = await _client.StartAnalyzeActionsAsync(
                new[] { text }, tasks);

            await operation.WaitForCompletionAsync();

            var result = new LanguageAnalysisResult();

            await foreach (var page in operation.Value)
            {
                foreach (var kpResult in page.ExtractKeyPhrasesResults)
                    foreach (var doc in kpResult.DocumentsResults)
                        result.KeyPhrases = doc.KeyPhrases.ToList();

                foreach (var entResult in page.RecognizeEntitiesResults)
                    foreach (var doc in entResult.DocumentsResults)
                        result.Entities = doc.Entities
                            .Select(e => $"{e.Text} ({e.Category})")
                            .ToList();

                foreach (var sentResult in page.AnalyzeSentimentResults)
                    foreach (var doc in sentResult.DocumentsResults)
                        result.Sentiment = doc.DocumentSentiment.Sentiment.ToString();
            }

            return result;
        }
    }

    public class LanguageAnalysisResult
    {
        public string Sentiment { get; set; } = string.Empty;
        public List<string> KeyPhrases { get; set; } = new();
        public List<string> Entities { get; set; } = new();
    }
}