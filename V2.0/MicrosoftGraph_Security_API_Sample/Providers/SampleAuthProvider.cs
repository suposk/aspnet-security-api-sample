// -----------------------------------------------------------------------
// <copyright file="SampleAuthProvider.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Identity.Client;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using MicrosoftGraph_Security_API_Sample.TokenStorage;
using Microsoft.Graph;
using Resources;

namespace MicrosoftGraph_Security_API_Sample.Providers
{
    public sealed class SampleAuthProvider : IAuthProvider
    {
        // Properties used to get and manage an access token.
        private static readonly SampleAuthProvider SampleInstance = new SampleAuthProvider();
        private string redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];
        private string appId = ConfigurationManager.AppSettings["ida:AppId"];
        private string appSecret = ConfigurationManager.AppSettings["ida:AppSecret"];
        private string scopes = ConfigurationManager.AppSettings["ida:GraphScopes"];

        private SampleAuthProvider()
        {
        }

        private SessionTokenCache TokenCache { get; set; }

        public static SampleAuthProvider Instance
        {
            get
            {
                return SampleInstance;
            }
        }

        // Gets an access token. First tries to get the token from the token cache.
        public async Task<string> GetUserAccessTokenAsync()
        {
            string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            HttpContextWrapper httpContext = new HttpContextWrapper(HttpContext.Current);
            TokenCache userTokenCache = new SessionTokenCache(signedInUserID, httpContext).GetMsalCacheInstance();
            //// var cachedItems = tokenCache.ReadItems(appId); // see what's in the cache

            ConfidentialClientApplication cca = new ConfidentialClientApplication(
                this.appId, 
                this.redirectUri,
                new ClientCredential(this.appSecret),
                userTokenCache,
                null);

            try
            {
                AuthenticationResult result = await cca.AcquireTokenSilentAsync(this.scopes.Split(new char[] { ' ' }), cca.Users.First());
                return result.AccessToken;
            }
            catch (Exception)
            {
                //// Unable to retrieve the access token silently.
                HttpContext.Current.Request.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties() { RedirectUri = "/" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);

                throw new ServiceException(
                    new Error
                    {
                        Code = GraphErrorCode.AuthenticationFailure.ToString(),
                        Message = Resource.Error_AuthChallengeNeeded,
                    });
            }
        }
    }
}
