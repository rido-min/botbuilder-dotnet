// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;

namespace Sample.Agentic
{
    public class CustomCloudAdapter(BotFrameworkAuthentication auth, ILogger logger)
        : CloudAdapter(auth, logger)
    {
    }
}
