using Azure;
using Azure.AI.Vision.ImageAnalysis;

namespace AiShowcase.Api.Services
{
    public class VisionService
    {
        private readonly ImageAnalysisClient _client;

        public VisionService(string endpoint, string apiKey)
        {
            _client = new ImageAnalysisClient(
                new Uri(endpoint),
                new AzureKeyCredential(apiKey)
            );
        }

        public async Task<VisionAnalysisResult> AnalyzeImageAsync(Stream imageStream)
        {
            var result = await _client.AnalyzeAsync(
                BinaryData.FromStream(imageStream),
                VisualFeatures.Caption |
                VisualFeatures.Tags |
                VisualFeatures.Objects
            );

            var analysis = result.Value;

            return new VisionAnalysisResult
            {
                Caption = analysis.Caption?.Text ?? "No caption generated",
                CaptionConfidence = analysis.Caption?.Confidence ?? 0,
                Tags = analysis.Tags?.Values
                    .Select(t => $"{t.Name} ({t.Confidence:P0})")
                    .ToList() ?? new List<string>(),
                Objects = analysis.Objects?.Values
                    .Select(o => o.Tags.FirstOrDefault()?.Name ?? "unknown")
                    .ToList() ?? new List<string>()
            };
        }
    }

    public class VisionAnalysisResult
    {
        public string Caption { get; set; } = string.Empty;
        public double CaptionConfidence { get; set; }
        public List<string> Tags { get; set; } = new();
        public List<string> Objects { get; set; } = new();
    }
}
