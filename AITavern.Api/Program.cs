using AITavern.Api.Config;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOptions<OpenAIConfig>("OpenAI");
builder.Services.AddTransient(sp => new OpenAIClient(sp.GetRequiredService<IOptions<OpenAIConfig>>().Value.Key));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = "https://blazingvtt.us.auth0.com/";
    options.Audience = "https://localhost:32768/";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGroup("tavern")
    .MapGet("/start", async (IServiceProvider sp) =>
    {
        var openApiClient = new OpenAIClient("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");
        var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, "Act as a tavern owner in a fantasy setting that has 3 barten"),
                new ChatMessage(ChatRole.User, "Give me the Tavern Name, Owner's name, 3 Bartender names, 3 Server Names and One Paragraph Description of the tavern formatted as JSON")
            };
        var chatCompletionsOptions = new ChatCompletionsOptions();
        chatCompletionsOptions.Messages.Add(messages[0]);
        chatCompletionsOptions.Messages.Add(messages[1]);
        var t = await openApiClient.GetChatCompletionsAsync("gpt-3.5-turbo", chatCompletionsOptions);
        messages.Add(t.Value.Choices.First().Message);
        chatCompletionsOptions.Messages.Add(t.Value.Choices.First().Message);
        messages.Add(new ChatMessage(ChatRole.User, "I walk up to the closest bar"));
        chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.User, "I walk up to the closest bar"));
        t = await openApiClient.GetChatCompletionsAsync("gpt-3.5-turbo", chatCompletionsOptions);
        messages.Add(t.Value.Choices.First().Message);
        return messages;
    })
    .WithName("Tavern")
    .WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
