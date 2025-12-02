using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Rido.BFLite.Compat.Adapter;
using Rido.BFLite.Core;
using Rido.BFLite.Core.Hosting;
using Sample.Echo;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();
builder.Services.AddBotApplication<BotApplication>();
builder.Services.AddSingleton<CompatBotAdapter>();
builder.Services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
builder.Services.AddTransient<IBot, EchoBot>();

var app = builder.Build();

app.MapPost("/api/messages", async (IBotFrameworkHttpAdapter adapter, IBot bot, HttpRequest request, HttpResponse response) =>
    await adapter.ProcessAsync(request, response, bot));

app.Run();
