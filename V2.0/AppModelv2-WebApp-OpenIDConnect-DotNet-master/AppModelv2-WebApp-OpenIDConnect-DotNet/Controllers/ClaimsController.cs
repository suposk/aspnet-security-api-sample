using AppModelv2_WebApp_OpenIDConnect_DotNet.Models;
using AppModelv2_WebApp_OpenIDConnect_DotNet.Providers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;

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
            var userClaims = User.Identity as System.Security.Claims.ClaimsIdentity;

            //You get the user’s first and last name below:
            ViewBag.Name = userClaims?.FindFirst("name")?.Value;

            // The 'preferred_username' claim can be used for showing the username
            ViewBag.Username = userClaims?.FindFirst("preferred_username")?.Value;

            // The subject/ NameIdentifier claim can be used to uniquely identify the user across the web
            ViewBag.Subject = userClaims?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // TenantId is the unique Tenant Id - which represents an organization in Azure AD
            ViewBag.TenantId = userClaims?.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

            return View();
        }

        public async Task Token()
        {
            var userClaims = User.Identity as System.Security.Claims.ClaimsIdentity;

            string accessToken = await SampleAuthProvider.Instance.GetUserAccessTokenAsync();
            ViewBag.accessToken = accessToken;
            
            var secureScores = await this.GetSecureScore(accessToken, "?$top=100");

            //return View();
        }


        public async Task<List<SecureScore>> GetSecureScore(string accessToken, string queryParameter)
        {
            try
            {
                string endpoint = "https://graph.microsoft.com/beta/security/securescores";
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
                                SecureScoreResult secureScoreResult = JsonConvert.DeserializeObject<SecureScoreResult>(result);
                                return secureScoreResult.Value;
                            }
                            else
                            {
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