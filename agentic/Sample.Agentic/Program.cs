// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Logging.Abstractions;
using Sample.Agentic;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

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
    new CustomCloudAdapter(
        sp.GetRequiredService<BotFrameworkAuthentication>(), 
        sp.GetService<ILogger<CustomCloudAdapter>>() ?? NullLogger<CustomCloudAdapter>.Instance));

builder.Services.AddTransient<IBot, EchoBot>();

var app = builder.Build();

app.MapPost("/api/messages", async (IBotFrameworkHttpAdapter adapter, IBot bot, HttpRequest request, HttpResponse response) =>
    await adapter.ProcessAsync(request, response, bot));

app.Run();
