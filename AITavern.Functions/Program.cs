using AITavern.Functions.Config;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(sp=> {
        sp.AddOptions<OpenAIConfig>();
        sp.AddTransient(sp => new OpenAIClient(sp.GetRequiredService<IOptions<OpenAIConfig>>().Value.Key));
     })
    .Build();


host.Run();
