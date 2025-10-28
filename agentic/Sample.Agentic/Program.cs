// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Sample.Agentic;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBotFrameworkAuthFromConfiguration(builder.Configuration);

builder.Services.AddTransient<IBot, EchoBot>();

var app = builder.Build();

app.MapPost("/api/messages", async (IBotFrameworkHttpAdapter adapter, IBot bot, HttpRequest request, HttpResponse response) =>
    await adapter.ProcessAsync(request, response, bot));

app.MapGet("/api/notify", async (IBotFrameworkHttpAdapter adapter, HttpRequest request, HttpResponse response) =>
{
    ConversationReference conversationReference = RicardoAgentConvRef();

    await ((BotAdapter)adapter).ContinueConversationAsync(
        botId: builder.Configuration["MicrosoftAppId"],
        reference: conversationReference,
        callback: async (turnContext, cancellationToken) =>
        {
            await turnContext.SendActivityAsync("This is a notification message.");
        },
        cancellationToken: default);

    await response.WriteAsync("Notification Sent");
});

app.Run();

static ConversationReference RicardoAgentConvRef()
{
    return new ConversationReference()
    {
        ChannelId = "msteams",
        ServiceUrl = "https://cosmic-int.botapi.skype.net/teams-int/5369a35c-46a5-4677-8ff9-2e65587654e7/",
        Conversation = new ConversationAccount
        {
            Id = "a:1_3ahtkiS6mv_JIqL34UrA0fcgrTlATx6u7KR0tiEik_DhHCB60jWI69l_C4ccPWB38INdl4OxNR5zJNPdtO7IM6nefqvJVRHLvL0dHQGXbZdAAoGN_-zOzIpIvCsHFlF"
        },
        User = new ChannelAccount
        {
            Id = "8:orgid:67c88439-0bdc-4345-b554-f4e5cb74d547",
            Name = "rmpablos",
            AadObjectId = "67c88439-0bdc-4345-b554-f4e5cb74d547"
        },
        Bot = new ChannelAccount
        {
            Id = "8:orgid:96a64abd-3267-4143-b9a5-194e6f96ef2b",
            Name = "RicardoAgent",
            Role = "agenticUser",
            Properties = 
            {
                ["agenticAppId"] = "5081ddac-3d33-4766-98fe-80c38c5ce554",
                ["agenticUserId"] = "96a64abd-3267-4143-b9a5-194e6f96ef2b",
                ["agenticAppBlueprintId"] = "9d17cc32-c91b-4368-8494-1b29ccb0dbcf",
            },
        }
    };
}

static ConversationReference RidobotlocalConvRef()
{
    return new ConversationReference()
    {
        ChannelId = "msteams",
        ServiceUrl = "https://smba.trafficmanager.net/amer/56653e9d-2158-46ee-90d7-675c39642038/",
        Conversation = new ConversationAccount
        {
            Id = "a:1_3ahtkiS6mv_JIqL34UrA0fcgrTlATx6u7KR0tiEik_DhHCB60jWI69l_C4ccPWB38INdl4OxNR5zJNPdtO7IM6nefqvJVRHLvL0dHQGXbZdAAoGN_-zOzIpIvCsHFlF"
        },
        User = new ChannelAccount
        {
            Id = "29:10n4Hk6RsMPuLvAxMNd2zEYU2w1dpvsiLC4QcffJ84rCMp_TKJO_dMzosR4d_K67eAumKyxTzXVYqHQWzRf2ukg",
            Name = "Rido",
            AadObjectId = "c5e99701-2a32-49c1-a660-4629ceeb8c61"
        },
        Bot = new ChannelAccount
        {
            Id = "28:aabdbd62-bc97-4afb-83ee-575594577de5",
            Name = "ridobotlocal"
        }
    };
}
