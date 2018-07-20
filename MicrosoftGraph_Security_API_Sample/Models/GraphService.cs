/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using Microsoft.Graph;
using MicrosoftGraph_Security_API_Sample.Helpers;
using Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MicrosoftGraph_Security_API_Sample.Models

{
    public class GraphService
    {
        public GraphServiceClient graphClient = null;

        /// <summary>
        /// Initialize the graphClient
        /// </summary>
        public GraphService()
        {
            graphClient = SDKHelper.GetAuthenticatedClient();
            if (graphClient != null)
            {
                graphClient.BaseUrl = ConfigurationManager.AppSettings["GraphBaseUrl"];
            }

        }

        /// <summary>
        /// Get the current user's email address from their profile.
        /// </summary>
        /// <returns>Email address of the signed in user</returns>
        public async Task<string> GetMyEmailAddress()
        {
            graphClient.BaseUrl = "https://graph.microsoft.com/beta";
            User me = await graphClient.Me.Request().Select("mail,userPrincipalName").GetAsync();
            graphClient.BaseUrl = ConfigurationManager.AppSettings["GraphBaseUrl"];
            return me.Mail ?? me.UserPrincipalName;
        }

        /// <summary>
        /// Get additional details about the user to help in investigating the alert
        /// </summary>
        /// <param name="principalName">User principal name</param>
        /// <param name="populatePicture"></param>
        /// <param name="populateManager"></param>
        /// <param name="populateDevices"></param>
        /// <returns>GraphUserModel</returns>
        public async Task<GraphUserModel> GetUserDetails(string principalName, bool populatePicture = false, bool populateManager = false, bool populateDevices = false)
        {
            GraphUserModel userModel = null;
            graphClient.BaseUrl = "https://graph.microsoft.com/beta";
            try
            {
                //var user = await graphClient.Users[principalName].Request().GetAsync();
                var user = await graphClient.Users.Request().Filter($"UserPrincipalName eq '{principalName}'").GetAsync();
                if (user.Count > 0)
                {
                    userModel = BuildGraphUserModel(user.CurrentPage[0]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (userModel == null)
            {
                return null;
            }

            try
            {
                if (populatePicture)
                {
                    var picture = await graphClient.Users[principalName].Photo.Content.Request().GetAsync();

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
                Console.WriteLine(ex.Message);
            }

            try
            {
                if (populateManager)
                {
                    var manager = await graphClient.Users[principalName].Manager.Request().GetAsync();
                    if (!string.IsNullOrEmpty(manager?.Id))
                    {
                        userModel.Manager = BuildGraphUserModel(await graphClient.Users[manager.Id].Request().GetAsync());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                if (populateDevices)
                {
                    var devices = await graphClient.Users[principalName].RegisteredDevices.Request().GetAsync();
                    if (devices != null)
                    {
                        userModel.RegisteredDevices = await Task.WhenAll(devices.Select(d => GetDeviceById(d.Id)));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                if (populateDevices)
                {
                    var devices = await graphClient.Users[principalName].OwnedDevices.Request().GetAsync();
                    if (devices != null)
                    {
                        userModel.OwnedDevices = await Task.WhenAll(devices.Select(d => GetDeviceById(d.Id)));
                    }
                }
            }
            catch { }

            graphClient.BaseUrl = ConfigurationManager.AppSettings["GraphBaseUrl"];
            return userModel;
        }

        /// <summary>
        /// Get additional details about the affected device
        /// </summary>
        /// <param name="id"></param>
        /// <returns>GraphDeviceModel</returns>
        public async Task<GraphDeviceModel> GetDeviceById(string id)
        {
            return BuildRegisteredOwnedeviceModel(await graphClient.Devices[id].Request().GetAsync());
        }

        /// <summary>
        /// Build Registered Owned Device Model
        /// </summary>
        /// <param name="device"></param>
        /// <returns>GraphDeviceModel</returns>
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
        /// <returns>GraphUserModel</returns>
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

            if (!Enum.TryParse<AlertStatus>(updateAlertModel.UpdateStatus, true, out var status))
            {
                throw new ArgumentOutOfRangeException(nameof(alert.Status));
            }

            alert.Status = status;

            if (!Enum.TryParse<AlertFeedback>(updateAlertModel.Feedback, true, out var feedback))
            {
                throw new ArgumentOutOfRangeException(nameof(alert.Feedback));
            }

            alert.Feedback = feedback;

            if (!string.IsNullOrEmpty(updateAlertModel.Comments))
            {
                alert.Comments = updateAlertModel.Comments;
            }           

            alert.AssignedTo = await GetMyEmailAddress();
            return await graphClient.Security.Alerts[alert.Id].Request().UpdateAsync(alert);
        }

        /// <summary>
        /// Get alert by Id
        /// </summary>
        /// <param name="alertId"></param>
        /// <returns>Alert</returns>
        public async Task<Alert> GetAlertById(string alertId)
        {
            if (string.IsNullOrEmpty(alertId))
            {
                return null;
            }

            Alert alert = await graphClient.Security.Alerts[alertId].Request().GetAsync();
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
        public async Task<ISecurityAlertsCollectionPage> GetAlerts(AlertFilter filters)
        {
            var filteredQuery = string.Empty;
            if (!string.IsNullOrEmpty(filters.AssignedToMe) && filters.AssignedToMe.Equals("AssignedToMe", StringComparison.InvariantCultureIgnoreCase))
            {
                filteredQuery += $"AssignedTo eq '{filters.Email}'";
            }

            if (filters.Category == null && filters.Provider == null && filters.Status == null && filters.Severity == null)
            {
                if (filters.Top != null)
                {
                    return await graphClient.Security.Alerts.Request().Top(filters.Top.Value).GetAsync();
                }
                else
                {
                    return await graphClient.Security.Alerts.Request().Top(1).GetAsync();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(filters.Category) && !filters.Category.Equals("All", StringComparison.InvariantCultureIgnoreCase))
                {
                    filteredQuery += (filteredQuery.Length == 0 ? $"Category eq '{filters.Category}'" : $" and Category eq '{filters.Category}'");
                }

                if (!string.IsNullOrEmpty(filters.Provider) && !filters.Provider.Equals("All", StringComparison.InvariantCultureIgnoreCase))
                {
                    filteredQuery += (filteredQuery.Length == 0 ? $"vendorInformation/provider eq '{filters.Provider}'" : $" and vendorInformation/provider eq '{filters.Provider}'");
                }

                if (!string.IsNullOrEmpty(filters.Status) && !filters.Status.Equals("All", StringComparison.InvariantCultureIgnoreCase))
                {
                    filteredQuery += (filteredQuery.Length == 0 ? $"Status eq '{filters.Status}'" : $" and Status eq '{filters.Status}'");
                }

                if (!string.IsNullOrEmpty(filters.Severity) && !filters.Severity.Equals("All", StringComparison.InvariantCultureIgnoreCase))
                {
                    filteredQuery += (filteredQuery.Length == 0 ? $"Severity eq '{filters.Severity}'" : $" and Severity eq '{filters.Severity}'");
                }

                if (!string.IsNullOrEmpty(filters.HostFqdn))
                {
                    filters.HostFqdn = filters.HostFqdn.ToLower();
                    filteredQuery += (filteredQuery.Length == 0 ? $"hostStates/any(a:a/fqdn eq '{filters.HostFqdn}')" : $" and hostStates/any(a:a/fqdn eq '{filters.HostFqdn}')");
                }

                if (!string.IsNullOrEmpty(filters.Upn))
                {
                    filters.Upn = filters.Upn.ToLower();
                    filteredQuery += (filteredQuery.Length == 0 ? $"userStates/any(a:a/userPrincipalName eq '{filters.Upn}')" : $" and userStates/any(a:a/userPrincipalName eq '{filters.Upn}')");
                }

                if (!filters.Top.HasValue)
                {
                    filters.Top = 1;
                }

                filters.FilteredQuery = filteredQuery;
                try
                {
                    return await graphClient.Security.Alerts.Request().Filter(filteredQuery).Top(filters.Top.Value).GetAsync();

                }
                catch (Exception e)
                {
                    return null;
                }
            }


        }

        /// <summary>
        /// Create webhook subscriptions in order to recieve the notifications
        /// </summary>
        /// <param name="filters"></param>
        /// <returns>The subscription object</returns>
        public async Task<Subscription> Subscribe(SubscriptionFilters filters)
        {
            var filteredQuery = string.Empty;
            var changeType = "updated";
            var resource = "/security/alerts";
            var expirationDate = DateTime.UtcNow.AddHours(1);
            Random Rand = new Random();
            var randno = Rand.Next(1, 100).ToString();
            var clientState = "IsgSdkSubscription" + randno;


            if (!string.IsNullOrEmpty(filters.SubscriptionCategory) && !filters.SubscriptionCategory.Equals("All", StringComparison.InvariantCultureIgnoreCase))
            {
                filteredQuery += (filteredQuery.Length == 0 ? $"Category eq '{filters.SubscriptionCategory}'" : $" and Category eq '{filters.SubscriptionCategory}'");
            }

            if (!string.IsNullOrEmpty(filters.SubscriptionProvider) && !filters.SubscriptionProvider.Equals("All", StringComparison.InvariantCultureIgnoreCase))
            {
                filteredQuery += (filteredQuery.Length == 0 ? $"vendorInformation/provider eq '{filters.SubscriptionProvider}'" : $" and vendorInformation/provider eq '{filters.SubscriptionProvider}'");
            }
            if (!string.IsNullOrEmpty(filters.SubscriptionSeverity) && !filters.SubscriptionSeverity.Equals("All", StringComparison.InvariantCultureIgnoreCase))
            {
                filteredQuery += (filteredQuery.Length == 0 ? $"Severity eq '{filters.SubscriptionSeverity}'" : $" and Severity eq '{filters.SubscriptionSeverity}'");
            }
            if (!string.IsNullOrEmpty(filters.SubscriptionHostFqdn))
            {
                filteredQuery += (filteredQuery.Length == 0 ? $"hostStates/any(a:a/fqdn eq '{filters.SubscriptionHostFqdn}')" : $" and hostStates/any(a:a/fqdn eq '{filters.SubscriptionHostFqdn}')");
            }

            if (!string.IsNullOrEmpty(filters.SubscriptionUpn))
            {
                filteredQuery += (filteredQuery.Length == 0 ? $"userStates/any(a:a/userPrincipalName eq '{filters.SubscriptionUpn}')" : $" and userStates/any(a:a/userPrincipalName eq '{filters.SubscriptionUpn}')");
            }

            resource += $"?$filter={filteredQuery}";

            Subscription subscription = new Subscription()
            {
                ChangeType = changeType,
                NotificationUrl = ConfigurationManager.AppSettings["ida:NotificationUrl"],
                Resource = resource,
                ExpirationDateTime = expirationDate,
                ClientState = clientState
            };

            return await graphClient.Subscriptions.Request().AddAsync(subscription);
        }

        /// <summary>
        /// List exisiting active subscriptions for this application
        /// </summary>
        /// <param name="filters"></param>
        /// <returns>The subscription collection page</returns>
        public async Task<IGraphServiceSubscriptionsCollectionPage> ListSubscriptions()
        {
            try
            {
                return await graphClient.Subscriptions.Request().GetAsync();
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
