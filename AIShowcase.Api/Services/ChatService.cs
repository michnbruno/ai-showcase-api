using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace AiShowcase.Api.Services
{
    public class ChatService
    {
        private readonly ChatClient _chatClient;

        public ChatService(string endpoint, string apiKey, string deploymentName)
        {
            var azureClient = new AzureOpenAIClient(
                new Uri(endpoint),
                new AzureKeyCredential(apiKey)
            );
            _chatClient = azureClient.GetChatClient(deploymentName);
        }

        public async Task<string> ChatAsync(string userMessage, string? systemPrompt = null)
        {
            var messages = new List<ChatMessage>();

            messages.Add(new SystemChatMessage(
                systemPrompt ?? "You are a helpful assistant for the Azure AI Showcase application. Be concise and informative."));

            messages.Add(new UserChatMessage(userMessage));

            var response = await _chatClient.CompleteChatAsync(messages);

            return response.Value.Content[0].Text;
        }
    }
}