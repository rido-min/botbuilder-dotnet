// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Security.Claims;

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Identity.Abstractions;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.Rest;

namespace Sample.Agentic
{
    public class AgenticAuthentication : BotFrameworkAuthentication
    {
        private readonly IConfiguration _config;

        public AgenticAuthentication(IConfiguration config)
        {
            _config = config;
        }

        public async override Task<AuthenticateRequestResult> AuthenticateRequestAsync(Activity activity, string authHeader, CancellationToken cancellationToken)
        {
            var agenticCredentials = new AgenticCredentialsProvider(_config);
            string? tenantId = _config["MicrosoftAppTenantId"];
            string scope = _config["MicrosoftAgentScope"] ?? throw new InvalidOperationException("MicrosoftAgentScope");

            // Fix for CS8602: Dereference of a possibly null reference.
            // Ensure activity.From.Properties is not null before accessing its members.

            if (activity?.Recipient == null)
            {
                throw new ArgumentNullException(nameof(activity.Recipient), "Activity.From cannot be null.");
            }

            if (activity.From.Properties == null)
            {
                throw new ArgumentNullException(nameof(activity.From.Properties), "Activity.From.Properties cannot be null.");
            }

            if (activity.From.Properties["agenticAppId"] == null)
            {
                throw new ArgumentNullException("agenticAppId", "agenticAppId property cannot be null.");
            }

            if (activity.From.Properties["agenticUserId"] == null)
            {
                throw new ArgumentNullException("agenticUserId", "agenticUserId property cannot be null.");
            }

            string fmiPath = activity.From.Properties["agenticAppId"]?.ToString() ?? throw new InvalidOperationException("agenticAppId property is not a string.");
            string agenticUserId = activity.From.Properties["agenticUserId"]?.ToString() ?? throw new InvalidOperationException("agenticUserId property is not a string.");

            string token = await agenticCredentials!.CreateAuthorizationHeaderForAppAsync(
                scope,
                new AuthorizationHeaderProviderOptions()
                {
                    AcquireTokenOptions = new AcquireTokenOptions()
                    {
                        FmiPath = fmiPath,
                        Tenant = tenantId,
                        ExtraHeadersParameters = new Dictionary<string, string>
                        {
                            { "x-ms-agentic-user-id", agenticUserId }
                        }
                    }
                },
                cancellationToken);

            JsonWebToken jsonWebToken = new JsonWebToken(token);
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(jsonWebToken.Claims);
            foreach (var claim in jsonWebToken.Claims)
            {
                if (claim.Type != "azp")
                {
                    claimsIdentity.AddClaim(new Claim(claim.Type, claim.Value));
                }
            }

            string audience = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "aud")?.Value ?? "unknown";

            return new AuthenticateRequestResult
            {
                Audience = audience,
                ClaimsIdentity = claimsIdentity,
                CallerId = "agentic-caller-id",
                ConnectorFactory = null // You can provide a custom ConnectorFactory if needed
            };
        }

        public override Task<AuthenticateRequestResult> AuthenticateStreamingRequestAsync(string authHeader, string channelIdHeader, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override ConnectorFactory CreateConnectorFactory(ClaimsIdentity claimsIdentity)
        {
            throw new NotImplementedException();
        }

        public override Task<UserTokenClient> CreateUserTokenClientAsync(ClaimsIdentity claimsIdentity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
