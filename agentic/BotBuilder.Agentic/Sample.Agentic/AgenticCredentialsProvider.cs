// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Security.Claims;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensibility;

namespace Sample.Agentic;

public class AgenticCredentialsProvider : IAuthorizationHeaderProvider
{
    private string _tenantId;
    private string _clientId;
    private string _secret;
    private string _agentScope;

    public AgenticCredentialsProvider(IConfiguration configuration)
    {
        _tenantId = configuration["MicrosoftAppTenantId"] ?? throw new ArgumentNullException("MicrosoftAppTenantId");
        _clientId = configuration["MicrosoftAppId"] ?? throw new ArgumentNullException("MicrosoftAppId");
        _secret = configuration["MicrosoftAppPassword"] ?? throw new ArgumentNullException("MicrosoftAppPassword");
        _agentScope = configuration["MicrosoftAgentScope"] ?? throw new ArgumentNullException("MicrosoftAgentScope");
    }

    public Task<string> CreateAuthorizationHeaderAsync(IEnumerable<string> scopes, AuthorizationHeaderProviderOptions? options = null, ClaimsPrincipal? claimsPrincipal = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<string> CreateAuthorizationHeaderForAppAsync(string scopes, AuthorizationHeaderProviderOptions? downstreamApiOptions = null, CancellationToken cancellationToken = default)
    {
        var authority = $"https://login.microsoftonline.com/{_tenantId}";
        var fmiPath = downstreamApiOptions?.AcquireTokenOptions?.FmiPath ?? throw new InvalidOperationException("FmiPath not set in Auth Options");
        var userId = downstreamApiOptions?.AcquireTokenOptions?.ExtraHeadersParameters?["x-ms-agentic-user-id"] ?? throw new InvalidOperationException("x-ms-agentic-user-id not set in ExtraHeadersParameters");

        IList<string> scopesAD = new[] { "api://AzureADTokenExchange/.default" };

        var agentAppClient = ConfidentialClientApplicationBuilder
           .Create(_clientId)
           .WithAuthority(authority)
           .WithClientSecret(_secret) // Use the managed identity token as the client assertion
           .Build();

        var result = await agentAppClient.AcquireTokenForClient(scopesAD)
            .WithFmiPath(fmiPath)
            .ExecuteAsync(cancellationToken);

#pragma warning disable CS0618 // Type or member is obsolete
        var agentIdentityClient = ConfidentialClientApplicationBuilder
                        .Create(fmiPath)
                        .WithClientAssertion(result.AccessToken)
                        .Build();
#pragma warning restore CS0618 // Type or member is obsolete

        var agentAuthResult = await agentIdentityClient.AcquireTokenForClient(scopesAD)
                        .WithTenantId(_tenantId)
                        .ExecuteAsync(cancellationToken);

#pragma warning disable CS0618 // Type or member is obsolete
        var cca = (IByUsernameAndPassword)ConfidentialClientApplicationBuilder
                                .Create(fmiPath)
                                .WithTenantId(_tenantId)
                                .WithClientAssertion(result.AccessToken)
                                .Build();
#pragma warning restore CS0618 // Type or member is obsolete
        
        //string[] apxScope = new string[] { "0d94caae-b412-4943-8a68-83135ad6d35f/.default" };
        //IList<string> apxScope = ["5a807f24-c9de-44ee-a3a7-329e88a00ffc/.default"];
        IList<string> apxScope = new string[] { _agentScope };

        var userToken = await cca
                        .AcquireTokenByUsernamePassword(apxScope, "no-user-name", "no_password")
                        .OnBeforeTokenRequest(async request =>
                        {
                            var userFicAssertion = agentAuthResult.AccessToken;
                            request.BodyParameters["user_federated_identity_credential"] = userFicAssertion;
                            request.BodyParameters["grant_type"] = "user_fic";
                            request.BodyParameters["user_id"] = userId;

                            // request.BodyParameters["client_assertion_type"] = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";

                            // remove the password
                            request.BodyParameters.Remove("password");
                            request.BodyParameters.Remove("username");

                            if (request.BodyParameters.TryGetValue("client_secret", out var secret)
                                    && secret.Equals("default", StringComparison.OrdinalIgnoreCase))
                            {
                                request.BodyParameters.Remove("client_secret");
                            }

                            await Task.CompletedTask;
                        }).ExecuteAsync(cancellationToken)
                        .ConfigureAwait(false);

        return userToken.AccessToken;
    }

    public async Task<string> CreateAuthorizationHeaderForUserAsync(IEnumerable<string> scopes2, AuthorizationHeaderProviderOptions? authorizationHeaderProviderOptions = null, ClaimsPrincipal? claimsPrincipal = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
