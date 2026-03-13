using AIShowcase.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIShowcase.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContentSafetyController : ControllerBase
{
    private readonly ContentSafetyService _service;

    public ContentSafetyController(ContentSafetyService service)
    {
        _service = service;
    }

    [HttpPost("analyze-text")]
    public async Task<IActionResult> AnalyzeText([FromBody] ContentSafetyTextRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest("Text is required.");

        var result = await _service.AnalyzeTextAsync(request.Text);
        return Ok(result);
    }

    //[HttpPost("analyze-image")]
    //public async Task<IActionResult> AnalyzeImage([FromBody] ImageAnalysisRequest request)
    //{
    //    if (string.IsNullOrWhiteSpace(request.ImageUrl))
    //        return BadRequest("ImageUrl is required.");

    //    var result = await _service.AnalyzeImageAsync(request.ImageUrl);
    //    return Ok(result);
    //}

    [HttpPost("analyze-image")]
    public IActionResult AnalyzeImage([FromBody] ContentSafetyImageRequest request)
    {
        return StatusCode(501, "Image analysis not supported in current SDK version.");
    }
}

public record ContentSafetyTextRequest(string Text);
public record ContentSafetyImageRequest(string ImageUrl);
