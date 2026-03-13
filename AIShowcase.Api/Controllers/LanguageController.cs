using AiShowcase.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiShowcase.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LanguageController : ControllerBase
    {
        private readonly LanguageService _languageService;

        public LanguageController(LanguageService languageService)
        {
            _languageService = languageService;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeText([FromBody] TextAnalysisRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Text))
                return BadRequest("No text provided.");

            var result = await _languageService.AnalyzeTextAsync(request.Text);

            return Ok(new
            {
                sentiment = result.Sentiment,
                keyPhrases = result.KeyPhrases,
                entities = result.Entities
            });
        }
    }

    public class TextAnalysisRequest
    {
        public string Text { get; set; } = string.Empty;
    }
}