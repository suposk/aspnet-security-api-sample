// -----------------------------------------------------------------------
// <copyright file="SDKHelper.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Net.Http.Headers;
using Microsoft.Graph;
using MicrosoftGraph_Security_API_Sample.Providers;

namespace MicrosoftGraph_Security_API_Sample.Helpers
{
    public class SDKHelper
    {   
        private static GraphServiceClient graphClient = null;

        // Get an authenticated Microsoft Graph Service client.
        public static GraphServiceClient GetAuthenticatedClient()
        {
            graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        string accessToken = await SampleAuthProvider.Instance.GetUserAccessTokenAsync();

                        // Append the access token to the request.
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);

                        // This header has been added to identify our sample in the Microsoft Graph service. If extracting this code for your project please remove.
                        requestMessage.Headers.Add("SampleID", "aspnet-connect-sample");
                    }));
            return graphClient;
        }

        public static void SignOutClient()
        {
            graphClient = null;
        }
    }
}