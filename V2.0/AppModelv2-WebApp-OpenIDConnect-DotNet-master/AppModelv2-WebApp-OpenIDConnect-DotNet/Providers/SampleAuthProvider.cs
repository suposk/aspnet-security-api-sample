using AppModelv2_WebApp_OpenIDConnect_DotNet.TokenStorage;
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

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Providers
{
    public sealed class SampleAuthProvider
    {
        // The Client ID is used by the application to uniquely identify itself to Azure AD.
        string clientId = ConfigurationManager.AppSettings["ClientId"];
        // RedirectUri is the URL where the user will be redirected to after they sign in.
        string redirectUri = ConfigurationManager.AppSettings["RedirectUri"];
        // Tenant is the tenant ID (e.g. contoso.onmicrosoft.com, or 'common' for multi-tenant)
        string tenant = ConfigurationManager.AppSettings["Tenant"];
        string graphScopes = System.Configuration.ConfigurationManager.AppSettings["GraphScopes"].ToLower();
        string appSecret = ConfigurationManager.AppSettings["ida:AppSecret"];


        private static readonly SampleAuthProvider SampleInstance = new SampleAuthProvider();
        private SampleAuthProvider()
        {
        }

        private SessionTokenCache TokenCache { get; set; }
        public string AccessToken { get; set; }

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
            ///

            ConfidentialClientApplication cca = null;

            //cca = new ConfidentialClientApplication(
            //    this.clientId,
            //    this.redirectUri,
            //    new ClientCredential(this.appSecret),
            //    userTokenCache,
            //    null);

            try
            {
                //string[] scopes = graphScopes.Split(new char[] { ' ' });
                //var scopes = new List<string> { "user.read" };
                var scopes = new List<string> { AppConstants.SCOPE };

                //string[] ascopes = graphScopes.Split(new char[] { ' ' });
                //var scopes = new List<string> { AppConstants.SCOPE };
                //scopes.AddRange(ascopes);

                IEnumerable<IAccount> accounts = await cca.GetAccountsAsync();
                IAccount firstAccount = accounts.FirstOrDefault();                
                if (firstAccount != null)
                {
                    var result = await cca.AcquireTokenSilent(graphScopes.Split(new char[] { ' ' }), firstAccount).ExecuteAsync();
                    return result.AccessToken;
                }
                else
                {
                    var result = await cca.AcquireTokenForClient(scopes).ExecuteAsync();
                    return result.AccessToken;
                }                
            }
            catch (Exception ex)
            {
                //// Unable to retrieve the access token silently.
                HttpContext.Current.Request.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties() { RedirectUri = "/" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);

                //throw new ServiceException(
                //    new Error
                //    {
                //        Code = GraphErrorCode.AuthenticationFailure.ToString(),
                //        Message = " Errror in Auth JANO",
                //    });

                throw;
            }
        }
    }
}