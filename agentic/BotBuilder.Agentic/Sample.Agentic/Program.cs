// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Sample.Agentic;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

builder.Services.AddSingleton<IBotFrameworkHttpAdapter>(provider =>
    new CloudAdapter(
        provider.GetRequiredService<IConfiguration>(),
        provider.GetRequiredService<IHttpClientFactory>(),
        provider.GetRequiredService<ILogger<CloudAdapter>>()));

builder.Services.AddTransient<IBot, EchoBot>();

var app = builder.Build();

app.MapPost("/api/messages", async (IBotFrameworkHttpAdapter adapter, IBot bot, HttpRequest request, HttpResponse response) =>
    await adapter.ProcessAsync(request, response, bot));

app.Run();



//string activityRAW = """
//        {
//      "type": "message",
//      "channelId": "msteams",
//      "text": "you said df, with \u2764\uFE0F at 4:54:17 PM",
//      "serviceUrl": "https://cosmic-int.botapi.skype.net/amer/5369a35c-46a5-4677-8ff9-2e65587654e7/",
//      "replyToId": "1761004454500",
//      "from": {
//        "id": "8:orgid:96a64abd-3267-4143-b9a5-194e6f96ef2b",
//        "name": "RicardoAgent",
//        "agenticUserId": "96a64abd-3267-4143-b9a5-194e6f96ef2b",
//        "agenticAppId": "5081ddac-3d33-4766-98fe-80c38c5ce554",
//        "agenticAppBlueprintId": "9d17cc32-c91b-4368-8494-1b29ccb0dbcf",
//        "callbackUri": "https://n3mzm86w-3978.usw2.devtunnels.ms/api/messages",
//        "tenantId": "5369a35c-46a5-4677-8ff9-2e65587654e7",
//        "role": "agenticUser"
//      },
//      "recipient": {
//        "id": "8:orgid:67c88439-0bdc-4345-b554-f4e5cb74d547",
//        "name": "rmpablos",
//        "aadObjectId": "67c88439-0bdc-4345-b554-f4e5cb74d547"
//      },
//      "conversation": {
//        "id": "19:67c88439-0bdc-4345-b554-f4e5cb74d547_96a64abd-3267-4143-b9a5-194e6f96ef2b@unq.gbl.spaces",
//        "conversationType": "personal",
//        "tenantId": "5369a35c-46a5-4677-8ff9-2e65587654e7"
//      }
//    }
//    """;

//Activity activity = new JsonSerializer().Deserialize<Activity>(new JsonTextReader(new StringReader(activityRAW)));
//Console.WriteLine(activity.From.Properties);
