using Azure;
using Azure.AI.ContentSafety;

namespace AIShowcase.Api.Services;

public class ContentSafetyService
{
    private readonly ContentSafetyClient _client;
    private readonly ILogger<ContentSafetyService> _logger;

    public ContentSafetyService(IConfiguration config, ILogger<ContentSafetyService> logger)
    {
        _logger = logger;
        var endpoint = config["ContentSafety:Endpoint"]!;
        var key = config["ContentSafety:Key"]!;
        _client = new ContentSafetyClient(new Uri(endpoint), new AzureKeyCredential(key));
    }

    public async Task<ContentSafetyResult> AnalyzeTextAsync(string text)
    {
        _logger.LogInformation("Analyzing text content safety...");

        var request = new AnalyzeTextOptions(text);
        var response = await _client.AnalyzeTextAsync(request);

        var categories = response.Value.CategoriesAnalysis
            .Select(c => new CategoryResult
            {
                Category = c.Category.ToString(),
                Severity = c.Severity ?? 0
            })
            .ToList();

        return new ContentSafetyResult
        {
            IsClean = categories.All(c => c.Severity == 0),
            Categories = categories,
            InputText = text
        };
    }

    public Task<ContentSafetyResult> AnalyzeImageAsync(string imageUrl)
    {
        throw new NotSupportedException(
            "Image URL analysis requires Azure.AI.ContentSafety 1.1+. " +
            "Upgrade NuGet package to enable this endpoint.");
    }
}

public class ContentSafetyResult
{
    public bool IsClean { get; set; }
    public string InputText { get; set; } = string.Empty;
    public List<CategoryResult> Categories { get; set; } = new();
}

public class CategoryResult
{
    public string Category { get; set; } = string.Empty;
    public int Severity { get; set; }
}