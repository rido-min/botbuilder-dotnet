// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Connector.Authentication;

namespace Sample.Agentic
{
    public class CustomAuthenticator()
        : AppCredentials(null, null, null), IAuthenticator
    {
        public async Task<AuthenticatorResult> GetTokenAsync(bool forceRefresh = false, string agenticId = "", string agenticUserId = "", string tenantId = "")
        {
            await Task.CompletedTask;
            return new AuthenticatorResult
            {
                AccessToken = "your_token",
                ExpiresOn = DateTime.UtcNow.AddHours(1)
            };
        }

        protected override Lazy<IAuthenticator> BuildIAuthenticator()
        {
            return new Lazy<IAuthenticator>(() => this);
        }
    }
}
