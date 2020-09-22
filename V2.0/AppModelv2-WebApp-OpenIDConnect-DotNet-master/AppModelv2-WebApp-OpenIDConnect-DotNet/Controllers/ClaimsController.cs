//using AppModelv2_WebApp_OpenIDConnect_DotNet.Models;
using AppModelv2_WebApp_OpenIDConnect_DotNet.Providers;
//using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
//using SecureScore = AppModelv2_WebApp_OpenIDConnect_DotNet.Models.SecureScore;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Controllers
{
    [Authorize]
    public class ClaimsController : Controller
    {
        /// <summary>
        /// Add user's claims to viewbag
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ReadValues();

            return View();
        }

        private void ReadValues()
        {
            var userClaims = User.Identity as System.Security.Claims.ClaimsIdentity;

            //You get the user’s first and last name below:
            ViewBag.Name = userClaims?.FindFirst("name")?.Value;

            // The 'preferred_username' claim can be used for showing the username
            ViewBag.Username = userClaims?.FindFirst("preferred_username")?.Value;

            // The subject/ NameIdentifier claim can be used to uniquely identify the user across the web
            ViewBag.Subject = userClaims?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // TenantId is the unique Tenant Id - which represents an organization in Azure AD
            ViewBag.TenantId = userClaims?.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;
        }

        //public async Task Token()
        public async Task<ActionResult> Token()
        {
            ReadValues();

            string accessToken = await SampleAuthProvider.Instance.GetUserAccessTokenAsync();
            ViewBag.AccessToken = accessToken;
            //string accessToken = SampleAuthProvider.Instance.AccessToken;

            var me = await this.GetMe(accessToken);
            var score = await this.GetScore(accessToken);
            var alers = await this.GetAlerts(accessToken, "?$top=1");
            //var secureScores = await this.GetSecureScore(accessToken, "");

            //var authenticationProvider = new DelegateAuthenticationProvider(
            //(requestMessage) =>
            //{
            //    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            //    return Task.FromResult(0);
            //});
            //var graphClient = new GraphServiceClient(authenticationProvider);

            //try
            //{

            //    var alerts = await graphClient.Security.Alerts
            //        .Request()
            //        .GetAsync();

            //    var scores = await graphClient.Security.SecureScores
            //        .Request()
            //        .Top(1)
            //        .GetAsync();
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine($"Error: {ex.Message} {ex?.InnerException}");
            //}
            return View("Index");
            //return View();
        }


        //public async Task<List<SecureScore>> GetAlerts(string accessToken, string queryParameter)
        public async Task<List<object>> GetAlerts(string accessToken, string queryParameter)
        {
            try
            {
                string endpoint = "https://graph.microsoft.com/v1.0/security/alerts"; queryParameter = string.Empty;

                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, endpoint + queryParameter))
                    {
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                        using (var response = await client.SendAsync(request))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                string result = await response.Content.ReadAsStringAsync();
                                //SecureScoreResult secureScoreResult = JsonConvert.DeserializeObject<SecureScoreResult>(result);
                                //return secureScoreResult.Value;
                                return null;
                            }
                            else
                            {
                                Debug.WriteLine($"Error: {response}");
                                return null;
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> GetMe(string accessToken, string endpoint = null)
        {
            try
            {
                string url = endpoint ?? "https://graph.microsoft.com/v1.0/me/";

                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                    {
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                        using (var response = await client.SendAsync(request))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                string result = await response.Content.ReadAsStringAsync();
                                return result;
                            }
                            else
                            {
                                Debug.WriteLine($"Error: {response}");
                                return null;
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        //public async Task<List<SecureScore>> GetScore(string accessToken, string endpoint = null)
        public async Task<List<object>> GetScore(string accessToken, string endpoint = null)
        {
            try
            {
                string url = endpoint ?? "https://graph.microsoft.com/beta/security/secureScores?$top=5";

                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                    {
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                        using (var response = await client.SendAsync(request))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                string result = await response.Content.ReadAsStringAsync();
                                //SecureScoreResult secureScoreResult = JsonConvert.DeserializeObject<SecureScoreResult>(result);
                                //return secureScoreResult.Value;
                                return null;
                            }
                            else
                            {
                                Debug.WriteLine($"Error: {response}");
                                return null;
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}