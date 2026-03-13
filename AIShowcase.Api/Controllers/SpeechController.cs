using AiShowcase.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiShowcase.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpeechController : ControllerBase
    {
        private readonly SpeechService _speechService;

        public SpeechController(SpeechService speechService)
        {
            _speechService = speechService;
        }

        [HttpPost("transcribe")]
        public async Task<IActionResult> Transcribe(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No audio file uploaded.");

            using var stream = file.OpenReadStream();
            var transcription = await _speechService.TranscribeAudioAsync(stream);

            return Ok(new
            {
                fileName = file.FileName,
                transcription
            });
        }
    }
}