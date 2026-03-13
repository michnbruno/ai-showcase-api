using AiShowcase.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiShowcase.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;

        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("message")]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
                return BadRequest("No message provided.");

            var response = await _chatService.ChatAsync(
                request.Message,
                request.SystemPrompt);

            return Ok(new
            {
                userMessage = request.Message,
                response,
                model = "gpt-4o-mini"
            });
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
        public string? SystemPrompt { get; set; }
    }
}
