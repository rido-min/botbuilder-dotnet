// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Sample.Agentic;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

//builder.Services.AddSingleton<BotFrameworkAuthentication>(sp =>
//{
//    return new ConfigurationBotFrameworkAuthentication(
//        builder.Configuration,
//        new CustomAuthenticatorFactory(),
//        null,
//        sp.GetService<IHttpClientFactory>());
//});
builder.Services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

builder.Services.AddSingleton(sp => 
    new CloudAdapter(
        sp.GetRequiredService<BotFrameworkAuthentication>(), 
        sp.GetService<ILogger<CloudAdapter>>()));

builder.Services.AddSingleton<IBotFrameworkHttpAdapter>(sp => sp.GetService<CloudAdapter>());

builder.Services.AddTransient<IBot, EchoBot>();

var app = builder.Build();

app.MapPost("/api/messages", async (IBotFrameworkHttpAdapter adapter, IBot bot, HttpRequest request, HttpResponse response) =>
    await adapter.ProcessAsync(request, response, bot));

app.Run();
