// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Connector.Authentication;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;

namespace Sample.Agentic
{
    public class CustomAuthenticator(IAuthorizationHeaderProvider tokenProvider, IConfiguration configuration)
        : AppCredentials(null, null, null), IAuthenticator
    {
        async Task<AuthenticatorResult> IAuthenticator.GetTokenAsync(bool forceRefresh, string agenticId, string agenticUserId, string tenantId, string agentBluePrintId, CancellationToken cancellationToken)
        {
            var scope = configuration["ToChannelFromBotOAuthScope"] ?? "https://api.botframework.com/.default";

            var options = new AuthorizationHeaderProviderOptions();

            string token;
            if (string.IsNullOrEmpty(agenticId))
            {
                token = await tokenProvider.CreateAuthorizationHeaderForAppAsync(scope, options, cancellationToken);
            }
            else
            {
                options.WithAgentUserIdentity(agenticId, Guid.Parse(agenticUserId));
                token = await tokenProvider.CreateAuthorizationHeaderAsync([scope], options, null, cancellationToken);
            }    

            return new AuthenticatorResult
            {
                AccessToken = token.Substring("Bearer ".Length),
                ExpiresOn = DateTime.UtcNow.AddHours(1)
            };
        }

        protected override Lazy<IAuthenticator> BuildIAuthenticator()
        {
            return new Lazy<IAuthenticator>(() => this);
        }
    }
}
