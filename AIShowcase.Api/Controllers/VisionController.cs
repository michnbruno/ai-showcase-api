using AiShowcase.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiShowcase.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VisionController : ControllerBase
    {
        private readonly VisionService _visionService;

        public VisionController(VisionService visionService)
        {
            _visionService = visionService;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No image file uploaded.");

            using var stream = file.OpenReadStream();
            var result = await _visionService.AnalyzeImageAsync(stream);

            return Ok(new
            {
                fileName = file.FileName,
                caption = result.Caption,
                captionConfidence = Math.Round(result.CaptionConfidence, 3),
                tags = result.Tags,
                objects = result.Objects
            });
        }
    }
}
