// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Connector.Authentication;
using Microsoft.Rest;

namespace Sample.Agentic
{
    public class CustomAuthenticatorFactory()
        : ServiceClientCredentialsFactory
    {
        public override Task<ServiceClientCredentials> CreateCredentialsAsync(string appId, string audience, string loginEndpoint, bool validateAuthority, CancellationToken cancellationToken)
        {
            var res = new CustomAuthenticator();
            return Task.FromResult<ServiceClientCredentials>(res);
        }

        public override async Task<bool> IsAuthenticationDisabledAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return false;
        }

        public override async Task<bool> IsValidAppIdAsync(string appId, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return true;
        }
    }
}
