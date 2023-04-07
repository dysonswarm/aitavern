using Azure.AI.OpenAI;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AITavern.Functions
{
    public class Function1
    {
        private readonly ILogger _logger;
        private readonly OpenAIClient _openAIClient;

        public Function1(ILoggerFactory loggerFactory, OpenAIClient openAIClient)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
            _openAIClient = openAIClient;
        }

        [Function("Tavern")]
        public async Task<List<ChatMessage>> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, "Act as a tavern owner in a fantasy setting"),
                new ChatMessage(ChatRole.User, "Give me the Tavern Name, Owner's name, 3 Bartender names, 3 Server Names and One Paragraph Description of the tavern formatted as JSON")
            };
            var chatCompletionsOptions = new ChatCompletionsOptions();
            chatCompletionsOptions.Messages.Add(messages[0]);
            chatCompletionsOptions.Messages.Add(messages[1]);
            var t = await _openAIClient.GetChatCompletionsAsync("gpt-3.5-turbo", chatCompletionsOptions);
            messages.Add(t.Value.Choices.First().Message);
            chatCompletionsOptions.Messages.Add(t.Value.Choices.First().Message);
            messages.Add(new ChatMessage(ChatRole.User, "I walk up to the closest bar"));
            chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.User, "I walk up to the closest bar"));
            t = await _openAIClient.GetChatCompletionsAsync("gpt-3.5-turbo", chatCompletionsOptions);
            messages.Add(t.Value.Choices.First().Message);
            return messages;
        }
    }
}
