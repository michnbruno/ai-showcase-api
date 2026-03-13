using AiShowcase.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiShowcase.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly DocumentIntelligenceService _documentService;

        public DocumentController(DocumentIntelligenceService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeDocument(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only PDF files are supported.");

            using var stream = file.OpenReadStream();
            var extractedText = await _documentService.AnalyzeDocumentAsync(stream);

            return Ok(new
            {
                fileName = file.FileName,
                characterCount = extractedText.Length,
                extractedText = extractedText
            });
        }
    }
}
