// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;
using Sample.Agentic;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddTokenAcquisition()
    .AddInMemoryTokenCaches()
    .AddHttpClient()
    .AddAgentIdentities();

builder.Services.Configure<MicrosoftIdentityApplicationOptions>(options =>
{
    options.Instance = "https://login.microsoftonline.com/";
    options.ClientId = builder.Configuration["MicrosoftAppId"];
    options.TenantId = builder.Configuration["MicrosoftAppTenantId"];
    options.ClientCredentials = new[]
    {
        new CredentialDescription()
        {
            SourceType = CredentialSource.ClientSecret,
            ClientSecret = builder.Configuration["MicrosoftAppPassword"]
        }
    };
});

builder.Services.AddSingleton<CustomAuthenticatorFactory>();
builder.Services.AddSingleton<BotFrameworkAuthentication>(sp =>
{
    return new ConfigurationBotFrameworkAuthentication(
        builder.Configuration,
        sp.GetService<CustomAuthenticatorFactory>(),
        null,
        sp.GetService<IHttpClientFactory>(),
        null);
});

builder.Services.AddSingleton<IBotFrameworkHttpAdapter>(sp => 
    new CloudAdapter(
        sp.GetRequiredService<BotFrameworkAuthentication>(), 
        sp.GetService<ILogger<CloudAdapter>>() ?? NullLogger<CloudAdapter>.Instance));

builder.Services.AddTransient<IBot, EchoBot>();

var app = builder.Build();

app.MapPost("/api/messages", async (IBotFrameworkHttpAdapter adapter, IBot bot, HttpRequest request, HttpResponse response) =>
    await adapter.ProcessAsync(request, response, bot));

app.Run();
