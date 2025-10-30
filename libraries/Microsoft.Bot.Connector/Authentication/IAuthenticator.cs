// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Bot.Connector.Authentication
{
    /// <summary>
    /// Contract for authentication classes that retrieve authentication tokens.
    /// </summary>
    public interface IAuthenticator
    {
        /// <summary>
        /// Acquires the security token.
        /// </summary>
        /// <param name="forceRefresh">Tells the method to acquire a new token regardless of expiration.</param>
        /// <param name="agenticAppId" >The agentic identifier.</param>
        /// <param name="agenticUserId" >The agentic user identifier.</param>
        /// <param name="tenantId" >The tenant identifier.</param>
        /// <param name="agenticBlueprintId" >The agent blueprint identifier.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task{AuthenticationResult}"/> object.</returns>
        public Task<AuthenticatorResult> GetTokenAsync(
            bool forceRefresh = false, 
            string agenticAppId = "", 
            string agenticUserId = "", 
            string tenantId = "", 
            string agenticBlueprintId = "", 
            CancellationToken cancellationToken = default);
    }
}
