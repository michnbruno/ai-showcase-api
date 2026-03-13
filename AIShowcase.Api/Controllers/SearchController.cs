using AiShowcase.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiShowcase.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly SearchService _searchService;

        public SearchController(SearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpPost("index")]
        public async Task<IActionResult> IndexDocument([FromBody] IndexDocumentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Content))
                return BadRequest("No content provided.");

            await _searchService.EnsureIndexExistsAsync();

            var id = Guid.NewGuid().ToString();
            await _searchService.IndexDocumentAsync(id, request.FileName ?? "unknown", request.Content);

            return Ok(new { id, message = "Document indexed successfully." });
        }

        [HttpGet("query")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest("No query provided.");

            var results = await _searchService.SearchAsync(q);

            return Ok(new
            {
                query = q,
                count = results.Count,
                results
            });
        }
    }

    public class IndexDocumentRequest
    {
        public string? FileName { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}