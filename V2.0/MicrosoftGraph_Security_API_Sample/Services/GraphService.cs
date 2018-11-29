// -----------------------------------------------------------------------
// <copyright file="GraphService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Graph;
using MicrosoftGraph_Security_API_Sample.Models.DomainModels;
using MicrosoftGraph_Security_API_Sample.Helpers;
using Resources;
using MicrosoftGraph_Security_API_Sample.Providers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft_Graph_SDK_ASPNET_Connect.Models;

namespace MicrosoftGraph_Security_API_Sample.Models
{
    public class GraphService
    {
        /// <summary>
        /// The Microsoft graph beta url version (User Picture details is in Beta)
        /// So we use GraphBetaUrl to retrieve additional details about the user
        /// </summary>
        public string GraphBetaUrl = "https://graph.microsoft.com/beta";

        /// <summary>
        /// The graphClient object
        /// </summary>
        private GraphServiceClient graphClient = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphService" /> class
        /// </summary>
        public GraphService()
        {
            this.graphClient = SDKHelper.GetAuthenticatedClient();

            if (this.graphClient != null)
            {
                this.GraphUrlVersion = ConfigurationManager.AppSettings["GraphUrlVersion"];
                this.GraphUrl = ConfigurationManager.AppSettings["GraphBaseUrl"] + this.GraphUrlVersion;
                this.graphClient.BaseUrl = this.GraphUrl;
            }
        }

        /// <summary>
        /// Gets the Microsoft graph base url 
        /// </summary>
        public string GraphUrl { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the Microsoft graph url version (v1.0 or beta) based on web.config file
        /// </summary>
        public string GraphUrlVersion { get; private set; } = string.Empty;

        /// <summary>
        /// Get the current user's email address from their profile.
        /// </summary>
        /// <returns>Email address of the signed in user</returns>
        public async Task<string> GetMyEmailAddress()
        {
            User me = await this.graphClient.Me.Request().Select("mail,userPrincipalName").GetAsync();
            return me.Mail ?? me.UserPrincipalName;
        }

        /// <summary>
        /// Get additional details about the user to help in investigating the alert
        /// </summary>
        /// <param name="principalName">User principal name</param>
        /// <param name="populatePicture"></param>
        /// <param name="populateManager"></param>
        /// <param name="populateDevices"></param>
        /// <returns>Graph User Model</returns>
        public async Task<GraphUserModel> GetUserDetails(string principalName, bool populatePicture = false, bool populateManager = false, bool populateDevices = false)
        {
            GraphUserModel userModel = null;
            this.graphClient.BaseUrl = this.GraphBetaUrl;
            try
            {              
                var user = await this.graphClient.Users.Request().Filter($"UserPrincipalName eq '{principalName}'").GetAsync();
                if (user.Count > 0)
                {
                    userModel = BuildGraphUserModel(user.CurrentPage[0]);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            if (userModel == null)
            {
                return null;
            }

            try
            {
                if (populatePicture)
                {
                    var picture = await this.graphClient.Users[principalName].Photo.Content.Request().GetAsync();

                    if (picture != null)
                    {
                        MemoryStream picture1 = (MemoryStream)picture;
                        string pic = "data:image/png;base64," + Convert.ToBase64String(picture1.ToArray(), 0, picture1.ToArray().Length);
                        userModel.Picture = pic;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            try
            {
                if (populateManager)
                {
                    var manager = await this.graphClient.Users[principalName].Manager.Request().GetAsync();
                    if (!string.IsNullOrEmpty(manager?.Id))
                    {
                        userModel.Manager = BuildGraphUserModel(await this.graphClient.Users[manager.Id].Request().GetAsync());
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            try
            {
                if (populateDevices)
                {
                    var devices = await this.graphClient.Users[principalName].RegisteredDevices.Request().GetAsync();
                    if (devices != null)
                    {
                        userModel.RegisteredDevices = await Task.WhenAll(devices.Select(d => this.GetDeviceById(d.Id)));
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            try
            {
                if (populateDevices)
                {
                    var devices = await this.graphClient.Users[principalName].OwnedDevices.Request().GetAsync();
                    if (devices != null)
                    {
                        userModel.OwnedDevices = await Task.WhenAll(devices.Select(d => this.GetDeviceById(d.Id)));
                    }
                }
            }
            catch
            {
            }

            this.graphClient.BaseUrl = this.GraphUrl;
            return userModel;
        }

        /// <summary>
        /// Get additional details about the affected device
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Graph Device Model</returns>
        public async Task<GraphDeviceModel> GetDeviceById(string id)
        {
            return BuildRegisteredOwnedeviceModel(await this.graphClient.Devices[id].Request().GetAsync());
        }

        /// <summary>
        /// Build Registered Owned Device Model
        /// </summary>
        /// <param name="device"></param>
        /// <returns>Graph Device Model</returns>
        private static GraphDeviceModel BuildRegisteredOwnedeviceModel(Device device)
        {
            if (device == null)
            {
                return null;
            }

            return new GraphDeviceModel
            {
                Id = device.Id,
                DisplayName = device.DisplayName,
                IsCompliant = device.IsCompliant,
                Os = device.OperatingSystem,
                OsVersion = device.OperatingSystemVersion,
                IsIntuneManaged = device.IsManaged,
                ApproximateLastSignIn = device.ApproximateLastSignInDateTime
            };
        }

        /// <summary>
        /// Build Graph User Model
        /// </summary>
        /// <param name="device"></param>
        /// <returns>Graph User Model</returns>
        private static GraphUserModel BuildGraphUserModel(User user)
        {
            if (user == null)
            {
                return null;
            }

            return new GraphUserModel
            {
                JobTitle = user.JobTitle,
                OfficeLocation = user.OfficeLocation,
                Email = user.UserPrincipalName,
                ContactVia = user.MobilePhone,
                DisplayName = user.DisplayName
            };
        }

        /// <summary>
        /// Update specific fields of alert
        /// </summary>
        /// <param name="alert"></param>
        /// <param name="updateAlertModel"></param>
        /// <returns>the updated alert</returns>
        public async Task<Alert> UpdateAlert(Alert alert, UpdateAlertModel updateAlertModel)
        {
            if (alert == null)
            {
                throw new ArgumentNullException(nameof(alert));
            }

            if (!Enum.TryParse<AlertStatus>(updateAlertModel.NewStatus, true, out var status))
            {
                throw new ArgumentOutOfRangeException(nameof(alert.Status));
            }

            alert.Status = status;

            if (!Enum.TryParse<AlertFeedback>(updateAlertModel.NewFeedback, true, out var feedback))
            {
                throw new ArgumentOutOfRangeException(nameof(alert.Feedback));
            }

            alert.Feedback = feedback;

            if (!string.IsNullOrEmpty(updateAlertModel.NewComments))
            {
                updateAlertModel.NewComments = updateAlertModel.NewComments.Replace("\r", string.Empty);
                char[] charSeperators = new char[] { '\n' };
                List<string> comments = updateAlertModel.NewComments.Split(charSeperators, StringSplitOptions.RemoveEmptyEntries).ToList();
                alert.Comments = comments;
            }
            else
            {
                alert.Comments = new List<string>();
            }

            alert.AssignedTo = await this.GetMyEmailAddress();
            return await this.graphClient.Security.Alerts[alert.Id].Request().UpdateAsync(alert);
        }

        /// <summary>
        /// Get alert by Id
        /// </summary>
        /// <param name="alertId"></param>
        /// <returns>Alert object</returns>
        public async Task<Alert> GetAlertById(string alertId)
        {
            if (string.IsNullOrEmpty(alertId))
            {
                return null;
            }

            Alert alert = await this.graphClient.Security.Alerts[alertId].Request().GetAsync();
            object aipData = null;
            alert.AdditionalData?.TryGetValue("AIPDataAccessState", out aipData);
            alert.AdditionalData.Clear();
            if (aipData != null)
            {
                alert.AdditionalData.Add("AIPDataAccessState", aipData);
            }
            return alert;
        }

        /// <summary>
        /// Get alerts based on the alert filters
        /// </summary>
        /// <param name="filters"></param>
        /// <returns>alerts matching the filtering criteria</returns>
        public async Task<Tuple<IEnumerable<Alert>, string>> GetAlerts(AlertFilterModel filters, Dictionary<string, string> odredByParams = null)
        {
            if (filters == null)
            {
                var result = await this.graphClient.Security.Alerts.Request().Top(filters.Top).GetAsync();
                return new Tuple<IEnumerable<Alert>, string>(result, string.Empty);
            }
            else
            {
                // Create filter query
                var filterQuery = GraphQueryProvider.GetQueryByAlertFilter(filters);

                var customOrderByParams = new Dictionary<string, string>();
                //// If there are no filters and there are no custom odredByParams (if specified only top X)
                if ((odredByParams == null || odredByParams.Count() < 1) && filters.Count < 1)
                {
                    //// Order by 1. Provider 2. CreatedDateTime (desc)
                    customOrderByParams.Add("vendorInformation/provider", "asc");
                    customOrderByParams.Add("createdDateTime", "desc");
                }
                else if (filters.Count >= 1 && filters.ContainsKey("createdDateTime"))
                {
                    customOrderByParams.Add("createdDateTime", "desc");
                }

                // Create request with filter and top X
                var request = this.graphClient.Security.Alerts.Request().Filter(filterQuery).Top(filters.Top);

                // Add order py params
                if (customOrderByParams.Count > 0)
                {
                    request = request.OrderBy(string.Join(", ", customOrderByParams.Select(param => $"{param.Key} {param.Value}")));
                }
                else if (odredByParams != null && odredByParams.Count() > 0)
                {
                    request = request.OrderBy(string.Join(", ", odredByParams.Select(param => $"{param.Key} {param.Value}")));
                }

                // Get alerts
                var result = await request.GetAsync();

                return new Tuple<IEnumerable<Alert>, string>(result, filterQuery);
            }
        }

        /// <summary>
        /// Create web hook subscriptions in order to receive the notifications
        /// </summary>
        /// <param name="filters"></param>
        /// <returns>The subscription object</returns>
        public async Task<Tuple<Subscription, string>> Subscribe(AlertFilterModel filters)
        {
            var changeType = "updated";
            var expirationDate = DateTime.UtcNow.AddHours(3);

            var randno = new Random().Next(1, 100).ToString();
            var clientState = "IsgSdkSubscription" + randno;

            var filteredQuery = GraphQueryProvider.GetQueryByAlertFilter(filters);

            var resource = filters.ContainsKey("AlertId") && filters.HasPropertyFilter("AlertId")
                ? $"/security/alerts/{filters.GetFirstPropertyFilter("AlertId").Value}" :
                $"/security/alerts{(!String.IsNullOrWhiteSpace(filteredQuery) ? $"?$filter={filteredQuery}" : string.Empty)}";

            Subscription subscription = new Subscription()
            {
                ChangeType = changeType,
                NotificationUrl = ConfigurationManager.AppSettings["ida:NotificationUrl"],
                Resource = resource,
                ExpirationDateTime = expirationDate,
                ClientState = clientState
            };

            var result = await this.graphClient.Subscriptions.Request().AddAsync(subscription);

            return new Tuple<Subscription, string>(result, filteredQuery);
        }

        /// <summary>
        /// List existing active subscriptions for this application
        /// </summary>
        /// <param name="filters"></param>
        /// <returns>The subscription collection page</returns>
        public async Task<IGraphServiceSubscriptionsCollectionPage> ListSubscriptions()
        {
            return await this.graphClient.Subscriptions.Request().GetAsync();
        }

        /// <summary>
        /// Get list of all existing providers
        /// </summary>
        /// <returns>List of providers</returns>
        public async Task<string[]> GetProviderList()
        {
            var top1AlertsResult = await this.GetAlerts(new AlertFilterModel());
            string[] empty = new string[1];
            return top1AlertsResult != null && top1AlertsResult.Item1 != null
                ? top1AlertsResult.Item1.Select(alert => alert.VendorInformation.Provider).ToArray()
                : empty;
        }

        /// <summary>
        /// Get information about alert statistics
        /// </summary>
        /// <returns>Statistics Data</returns>
        public async Task<AlertStatisticModel> GetStatisticAsync(int topAlertAmount)
        {
            // Select all unresolved alerts 
            var filter = new AlertFilterModel { Top = topAlertAmount };
            filter.Add(
                "Status",
                new List<AlertFilterProperty>()
                {
                    {
                        new AlertFilterProperty(new AlertFilterPropertyDescription("Status", AlertFilterOperator.NotEquals), "resolved") }
                });
            var unresolvedAlerts = await this.GetAlerts(filter);

            //// Select all secure scores

            string accessToken = await SampleAuthProvider.Instance.GetUserAccessTokenAsync();
            var secureScores = await this.GetSecureScore(accessToken, "?$top=100");

            var latestSecureScore = secureScores.OrderByDescending(rec => rec.CreatedDateTime).FirstOrDefault();

            var secureScoreModel = latestSecureScore != null
                ? new SecureScoreStatisticModel { Current = latestSecureScore.CurrentScore ?? 0, Max = latestSecureScore.MaxScore ?? 0 }
                : null;

            var activeAlerts = new Dictionary<string, int>();
            var usersAlerts = new StatisticCollectionModel<SeveritySortOrder>();
            var hostsAlerts = new StatisticCollectionModel<SeveritySortOrder>();
            var providersAlerts = new StatisticCollectionModel<SeveritySortOrder>();
            var ipAlerts = new StatisticCollectionModel<SeveritySortOrder>();

            if (unresolvedAlerts != null && unresolvedAlerts.Item1 != null)
            {
                foreach (var alert in unresolvedAlerts.Item1)
                {
                    // Calculate active alerts
                    if (!activeAlerts.ContainsKey(alert.Severity.ToString()))
                    {
                        activeAlerts.Add(alert.Severity.ToString(), 1);
                    }
                    else
                    {
                        ++activeAlerts[alert.Severity.ToString()];
                    }

                    // Calculate users with the most alerts
                    var userPrincipalName = alert.UserStates?.FirstOrDefault()?.UserPrincipalName;
                    if (!string.IsNullOrWhiteSpace(userPrincipalName))
                    {
                        usersAlerts.Add(userPrincipalName, alert.Severity.ToString());
                    }

                    // Calculate destination ip address with the most alerts
                    var ipAddress = alert.NetworkConnections?.FirstOrDefault()?.DestinationAddress;
                    if (!string.IsNullOrWhiteSpace(ipAddress))
                    {
                        ipAlerts.Add(ipAddress, alert.Severity.ToString());
                    }

                    // Calculate hosts with the most alerts
                    var hostName = alert.HostStates?.FirstOrDefault()?.Fqdn;
                    if (!string.IsNullOrWhiteSpace(hostName))
                    {
                        hostsAlerts.Add(hostName, alert.Severity.ToString());
                    }

                    // Calculate providers with the most alerts
                    var provider = alert.VendorInformation.Provider;
                    if (!string.IsNullOrWhiteSpace(provider))
                    {
                        providersAlerts.Add(provider, alert.Severity.ToString());
                    }
                }
            }

            // Get top of the sorted users with the most alerts
            var sortedTopUserAlertsWithPrincipalNames = usersAlerts.GetSortedTopValues(4);
            var sortedTopUserAlert = new Dictionary<KeyValuePair<string, string>, Dictionary<SeveritySortOrder, int>>();

            // Get additional information about each user from top list
            var users = await this.GetUserDisplayNames(sortedTopUserAlertsWithPrincipalNames.Select(rec => rec.Key));

            //// Replaced UserPrincipalName to DisplayName if it is possible
            if (users != null)
            {
                foreach (var rec in sortedTopUserAlertsWithPrincipalNames)
                {
                    var newKey = users.ContainsKey(rec.Key) && !users[rec.Key].Equals(rec.Key, StringComparison.CurrentCultureIgnoreCase) ? users[rec.Key] : rec.Key;
                    sortedTopUserAlert.Add(new KeyValuePair<string, string>(newKey, rec.Key), rec.Value);
                }
            }

            return new AlertStatisticModel
            {
                ActiveAlerts = activeAlerts,
                SecureScore = secureScoreModel,
                UsersWithTheMostAlerts = sortedTopUserAlert,
                HostsWithTheMostAlerts = hostsAlerts.GetSortedTopValues(4)
                    .Select(rec => new KeyValuePair<KeyValuePair<string, string>, Dictionary<SeveritySortOrder, int>>(
                        new KeyValuePair<string, string>(
                            rec.Key.Split('.').FirstOrDefault() ?? rec.Key,
                            rec.Key),
                        rec.Value)).ToDictionary(rec => rec.Key, rec => rec.Value),
                ProvidersWithTheMostAlerts = providersAlerts.GetSortedTopValues(4).Select(rec => new KeyValuePair<KeyValuePair<string, string>, Dictionary<SeveritySortOrder, int>>(
                        new KeyValuePair<string, string>(
                            rec.Key,
                            rec.Key),
                        rec.Value)).ToDictionary(rec => rec.Key, rec => rec.Value),
                IPWithTheMostAlerts = ipAlerts.GetSortedTopValues(4)
                    .Select(rec => new KeyValuePair<KeyValuePair<string, string>, Dictionary<SeveritySortOrder, int>>(
                        new KeyValuePair<string, string>(
                            rec.Key.Split('.').FirstOrDefault() ?? rec.Key,
                            rec.Key),
                        rec.Value)).ToDictionary(rec => rec.Key, rec => rec.Value),
            };
        }

        /// <summary>
        /// List existing secure scores
        /// Secure scores is still in Beta. So This sample uses REST queries to get the secure scores, since the official SDK is only available for workloads in V1.0
        /// </summary>
        /// <returns>List of secure scores</returns>
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

        /// <summary>
        /// Get secure score control profiles
        /// Secure score control profiles is still in Beta. 
        /// So This sample uses REST queries to get the secure score control profiles, since the official SDK is only available for workloads in V1.0
        /// </summary>
        /// <returns>List of secure scores</returns>
        public async Task<IEnumerable<SecureScoreControlProfileModel>> GetSecureScoreControlProfiles(string accessToken)
        {
            List<SecureScoreControlProfile> secureScoreControlProfiles = null;
            try
            {
                string endpoint = "https://graph.microsoft.com/beta/security/securescorecontrolprofiles";
                string queryParameter = "?$top=10";

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
                                SecureScoreControlProfileResult secureScoreResult = JsonConvert.DeserializeObject<SecureScoreControlProfileResult>(result);
                                secureScoreControlProfiles = secureScoreResult.Value;
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
            var secureProfiles = secureScoreControlProfiles?.Select(profile => new SecureScoreControlProfileModel
            {
                ControlCategory = profile.ControlCategory,
                Title = profile.AdditionalData.ContainsKey("title") ? profile.AdditionalData["title"].ToString() : string.Empty,
                Rank = profile.AdditionalData.ContainsKey("rank") ? (int?)int.Parse(profile.AdditionalData["rank"].ToString()) : null,
                ImplementationCost = profile.ImplementationCost,
                MaxScore = profile.MaxScore,
                UserImpact = profile.UserImpact,
                ActionUrl = profile.ActionUrl,
                Deprecated = profile.Deprecated,
                LastModifiedDateTime = profile.LastModifiedDateTime,
                AzureTenantId = profile.AzureTenantId,
                Remediation = profile.Remediation,
                RemediationImpact = profile.RemediationImpact,
                Service = profile.Service,
                TenantNote = profile.TenantNote,
                TenantSetState = profile.TenantSetState,
                Threats = profile.Threats,
                Tier = profile.Tier,
                SecureStateUpdates = profile.AdditionalData.ContainsKey("controlStateUpdates")
                            ? JsonConvert.DeserializeObject<IEnumerable<ControlStateUpdateModel>>(profile.AdditionalData["controlStateUpdates"].ToString())
                            : Enumerable.Empty<ControlStateUpdateModel>()
            })
                        ?? Enumerable.Empty<SecureScoreControlProfileModel>();
            this.graphClient.BaseUrl = this.GraphUrl;

            var usersAssignedTo = secureProfiles.SelectMany(
                profile => profile.SecureStateUpdates.SelectMany(
                    update => new List<string>() { update.UpnAssignedTo, update.UpnUpdatedBy }).Where(upn => !string.IsNullOrWhiteSpace(upn)).Distinct());
            var users = await this.GetUserDisplayNames(usersAssignedTo);
            return secureProfiles;
        }

        /// <summary>
        /// Get user display names for a list of user principal names
        /// </summary>
        /// <returns>dictionary that maps user name to user principal name</returns>
        private async Task<Dictionary<string, string>> GetUserDisplayNames(IEnumerable<string> upns)
        {
            //// Create filter for query
            var filterRequest = string.Join(
                $" {AlertFilterOperator.Or} ",
                upns.Select(upn => $"UserPrincipalName {AlertFilterOperator.Equals} '{upn}'"));
            Dictionary<string, string> dict = (await this.graphClient.Users.Request().Filter(filterRequest).GetAsync()).ToDictionary(user => user.UserPrincipalName, user => user.DisplayName, StringComparer.InvariantCultureIgnoreCase);         
            //// Get additional information about each user from top list
            return dict;
        }
    }
}
