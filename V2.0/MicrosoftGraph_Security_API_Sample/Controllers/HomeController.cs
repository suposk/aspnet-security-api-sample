// -----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using System.Web.Mvc;
using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using Microsoft.Graph;
using MicrosoftGraph_Security_API_Sample.Helpers;
using MicrosoftGraph_Security_API_Sample.Models.DomainModels;
using MicrosoftGraph_Security_API_Sample.Models;
using Resources;
using Newtonsoft.Json;
using MicrosoftGraph_Security_API_Sample.Models.ViewModels;
using MicrosoftGraph_Security_API_Sample.Filters;
using MicrosoftGraph_Security_API_Sample.Providers;

namespace MicrosoftGraph_Security_API_Sample.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// The security scopes needed to view and update alerts
        /// </summary>
        private static List<string> securityScopes = new List<string>() { "SecurityEvents.Read.All", "SecurityEvents.ReadWrite.All" };

        /// <summary>
        /// The graph service object
        /// </summary>
        private GraphService graphService = new GraphService();

        /// <summary>
        /// The graph url version used to make the queries
        /// </summary>
        private string graphUrlVersion = ConfigurationManager.AppSettings["GraphUrlVersion"];

        private AlertStatisticViewModel AlertStatistic
        {
            get => this.Session["AlertStatistic"] as AlertStatisticViewModel;
            set => this.Session["AlertStatistic"] = value;
        }

        private IEnumerable<AlertResultItemModel> AlertSearchResult
        {
            get => this.Session["AlertSearchResult"] as IEnumerable<AlertResultItemModel>;
            set => this.Session["AlertSearchResult"] = value;
        }

        private ResultQueriesViewModel ResultQueries
        {
            get => this.Session["ResultQueries"] as ResultQueriesViewModel;
            set => this.Session["ResultQueries"] = value;
        }

        private AlertFilterViewModel AlertFilters
        {
            get => this.Session["AlertFilters"] as AlertFilterViewModel;
            set => this.Session["AlertFilters"] = value;
        }

        private UpdateAlertResultModel UpdateAlertResult
        {
            get => this.Session["UpdateAlertResult"] as UpdateAlertResultModel;
            set => this.Session["UpdateAlertResult"] = value;
        }

        private AlertDetailsViewModel CurrentAlert
        {
            get => this.Session["CurrentAlert"] as AlertDetailsViewModel;
            set => this.Session["CurrentAlert"] = value;
        }

        private string[] ProviderList
        {
            get => this.Session["ProviderList"] as string[];
            set => this.Session["ProviderList"] = value;
        }

        private IEnumerable<SubscriptionResultModel> SubscriptionCollection
        {
            get => this.Session["SubscriptionCollection"] as IEnumerable<SubscriptionResultModel>;
            set => this.Session["SubscriptionCollection"] = value;
        }

        private SubscriptionResultModel CreateSubscriptionResult
        {
            get => this.Session["CreateSubscriptionResult"] as SubscriptionResultModel;
            set => this.Session["CreateSubscriptionResult"] = value;
        }

        private SecureScoreDetailsViewModel SecureDetails
        {
            get => this.Session["SecureDetails"] as SecureScoreDetailsViewModel;
            set => this.Session["SecureDetails"] = value;
        }

        public async Task<ActionResult> Index()
        {
            // Reset all states
            this.AlertStatistic = null;
            this.AlertSearchResult = null;
            this.ResultQueries = null;
            this.AlertFilters = new AlertFilterViewModel { Top = 1, Filters = new AlertFilterCollection() };
            this.UpdateAlertResult = null;
            this.CurrentAlert = null;
            this.ProviderList = null;
            this.SubscriptionCollection = null;
            this.CreateSubscriptionResult = null;
            this.SecureDetails = null;
            if (!Request.IsAuthenticated)
            {
                this.Session["ProviderList"] = new[] { "All" };
            }
            else
            {
                if (Startup.UserScopes != null && Startup.UserScopes.Any() && !securityScopes.All(permission => Startup.UserScopes.Contains(permission)))
                {
                    return this.View("AdminConsent");
                }
                else
                {
                    var statisticModel = await this.graphService.GetStatisticAsync(200);
                    this.AlertStatistic = new AlertStatisticViewModel
                    {
                        ActiveAlerts = statisticModel.ActiveAlerts.OrderBy(rec => (int)Enum.Parse(typeof(SeveritySortOrder), rec.Key)),
                        SecureScore = statisticModel.SecureScore,
                        UsersWithTheMostAlerts = statisticModel.UsersWithTheMostAlerts,
                        HostsWithTheMostAlerts = statisticModel.HostsWithTheMostAlerts,
                        ProvidersWithTheMostAlerts = statisticModel.ProvidersWithTheMostAlerts,
                        IPWithTheMostAlerts = statisticModel.IPWithTheMostAlerts
                    };
                }
            }
            await this.CheckProviderList();
            return this.View("Graph");
        }

        /// <summary>
        ///  Get the provider list
        /// </summary>
        /// <returns></returns>
        private async Task SetProviderList()
        {
            var providers = new string[] { "All" };

            this.ProviderList = User.Identity.IsAuthenticated
                ? providers.Concat(await this.graphService.GetProviderList()).ToArray()
                : providers;
        }

        /// <summary>
        ///  Checks the provider list
        /// </summary>
        /// <returns></returns>
        private async Task CheckProviderList()
        {
            if (this.ProviderList == null
                || (User.Identity.IsAuthenticated && this.ProviderList.Length == 1 && "All".Equals(this.ProviderList[0])))
            {
                await this.SetProviderList();
            }
        }

        /// <summary>
        ///  Get the alerts based on filters
        /// </summary>
        /// <param name="alertFilter"></param>
        /// <returns></returns>
        [Authorize]
        [MultiButton(MatchFormKey = "Action", MatchFormValue = "GetAlerts")]
        public async Task<ActionResult> GetAlerts(AlertFilterViewModel viewAlertFilter)
        {
            try
            {
                await this.CheckProviderList();
                this.CurrentAlert = null;
                this.UpdateAlertResult = null;
                this.SubscriptionCollection = null;
                this.SecureDetails = null;

                var filter = new AlertFilterModel(viewAlertFilter);

                var securityAlertsResult = await this.graphService.GetAlerts(filter);
                var filterQuery = securityAlertsResult?.Item2 ?? string.Empty;

                // Generate queries
                var sdkQueryBuilder = new StringBuilder();
                var restQueryBuilder = new StringBuilder();
                sdkQueryBuilder.Append("await graphClient.Security.Alerts.Request()");
                if (!string.IsNullOrEmpty(filterQuery))
                {
                    sdkQueryBuilder.Append($".Filter(\"{filterQuery}\")");
                }
                sdkQueryBuilder.Append($".Top({viewAlertFilter.Top}).GetAsync()");

                if (!string.IsNullOrEmpty(filterQuery))
                {
                    restQueryBuilder.Append($"<a href=\"https://developer.microsoft.com/en-us/graph/graph-explorer?request=security/alerts?$filter={HttpUtility.UrlEncode(filterQuery)}%26$top={viewAlertFilter.Top}&&method=GET&version={graphUrlVersion}&GraphUrl=https://graph.microsoft.com\" target=\"_blank\">https://graph.microsoft.com/{graphUrlVersion}/security/alerts?");

                    restQueryBuilder.Append($"$filter={HttpUtility.UrlEncode(filterQuery)}&");
                    restQueryBuilder.Append($"$top={viewAlertFilter.Top}</a>");
                }
                else
                {
                    restQueryBuilder.Append($"<a href=\"https://developer.microsoft.com/en-us/graph/graph-explorer?request=security/alerts?$top={viewAlertFilter.Top}&&method=GET&version={graphUrlVersion}&GraphUrl=https://graph.microsoft.com\" target=\"_blank\">https://graph.microsoft.com/{graphUrlVersion}/security/alerts?");
                    restQueryBuilder.Append($"$top={viewAlertFilter.Top}</a>");
                }

                // Save alerts to session
                this.AlertSearchResult = securityAlertsResult?.Item1?.Select(sa => new AlertResultItemModel
                {
                    Id = sa.Id,
                    Title = sa.Title,
                    Status = sa.Status,
                    Provider = sa.VendorInformation?.Provider,
                    CreatedDateTime = sa.CreatedDateTime,
                    Severity = sa.Severity.ToString(),
                    Category = sa.Category
                }) ?? Enumerable.Empty<AlertResultItemModel>();

                // Save queries to session
                this.ResultQueries = new ResultQueriesViewModel
                {
                    SDKQuery = sdkQueryBuilder.ToString(),
                    RESTQuery = restQueryBuilder.ToString()
                };

                this.AlertFilters = viewAlertFilter;

                return this.View("Graph");
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded)
                {
                    return new EmptyResult();
                }
                return this.RedirectToAction("Index", "Error", new { message = Resource.Error_Message + Request.RawUrl + ": " + se.Error.Message });
            }
        }

        /// <summary>
        /// Create the subscription based on the subscription filters
        /// </summary>
        /// <param name="subscriptionFilters"></param>
        /// <returns></returns>
        [Authorize]
        [MultiButton(MatchFormKey = "Action", MatchFormValue = "Subscribe")]
        public async Task<ActionResult> Subscribe(AlertFilterViewModel viewAlertFilter)
        {
            await this.CheckProviderList();

            this.AlertSearchResult = null;
            this.UpdateAlertResult = null;
            this.SecureDetails = null;

            var actViewAlertFilter = viewAlertFilter;
            if (this.CurrentAlert != null)
            {
                // Replace filter
                var filters = new AlertFilterCollection();
                filters.Add("alert:id", this.CurrentAlert.Id);
                actViewAlertFilter = new AlertFilterViewModel { Filters = filters };
                this.CurrentAlert = null;
            }

            this.AlertFilters = actViewAlertFilter;

            try
            {
                if (actViewAlertFilter.Filters.GetFilterValue("alert:category").Equals("All")
                    && actViewAlertFilter.Filters.GetFilterValue("vendor:provider").Equals("All")
                    && actViewAlertFilter.Filters.GetFilterValue("alert:severity").Equals("All")
                    && string.IsNullOrWhiteSpace(actViewAlertFilter.Filters.GetFilterValue("host:hostFqdn"))
                    && string.IsNullOrWhiteSpace(actViewAlertFilter.Filters.GetFilterValue("user:upn"))
                    && string.IsNullOrWhiteSpace(actViewAlertFilter.Filters.GetFilterValue("AlertId")))
                {
                    var subscriptionResultModel = new SubscriptionResultModel()
                    {
                        Error = "Please select at least one property/criterion for subscribing to alert notifications"
                    };
                    this.CreateSubscriptionResult = subscriptionResultModel;
                }
                else
                {
                    var filter = new AlertFilterModel(actViewAlertFilter);
                    var createSubscriptionResult = await this.graphService.Subscribe(filter);
                    var subscription = createSubscriptionResult.Item1;

                    var sdkQueryBuilder = new StringBuilder();
                    var restQueryBuilder = new StringBuilder();

                    sdkQueryBuilder.Append($"graphClient.Subscriptions.Request().AddAsync(subscription)");

                    restQueryBuilder.Append($"POST <a>https://graph.microsoft.com/{graphUrlVersion}/subscriptions</a>");
                    restQueryBuilder.Append("<br />");
                    restQueryBuilder.Append($"Request Body: ResourceUri = {subscription.Resource}; ExpirationDateTime = {subscription.ExpirationDateTime}; ");

                    // Save result of subscription creating
                    this.CreateSubscriptionResult = subscription != null
                        ? new SubscriptionResultModel()
                        {
                            Id = subscription.Id,
                            Resource = subscription.Resource,
                            NotificationUrl = subscription.NotificationUrl,
                            ExpirationDateTime = subscription.ExpirationDateTime,
                            ChangeType = subscription.ChangeType,
                            ClientState = subscription.ClientState
                        }
                        : null;

                    // Reload all active subscription
                    if (this.SubscriptionCollection != null)
                    {
                        var subscriptions = await this.graphService.ListSubscriptions();
                        this.SubscriptionCollection = subscriptions?.Select(sa => new SubscriptionResultModel
                        {
                            Id = sa.Id,
                            Resource = sa.Resource,
                            ExpirationDateTime = sa.ExpirationDateTime,
                            ClientState = sa.ClientState,
                            NotificationUrl = sa.NotificationUrl
                        }) ?? Enumerable.Empty<SubscriptionResultModel>();
                    }

                    // Save queries to session
                    this.ResultQueries = new ResultQueriesViewModel
                    {
                        SDKQuery = sdkQueryBuilder.ToString(),
                        RESTQuery = restQueryBuilder.ToString()
                    };
                }

                return this.View("Graph");
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded)
                {
                    return new EmptyResult();
                }

                return this.RedirectToAction("Index", "Error", new
                {
                    message = Resource.Error_Message + Request.RawUrl + ": " + se.Error.Message
                });
            }
            catch (Exception ex)
            {
                return this.RedirectToAction("Index", "Error", new { message = Resource.Error_Message + Request.RawUrl + ": " + ex.Message });
            }
        }

        /// <summary>
        /// Updates specific fields of alert
        /// </summary>
        /// <param name="updateAlertModel"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> UpdateAlert(UpdateAlertModel updateAlertModel)
        {
            try
            {
                await this.CheckProviderList();
                this.SubscriptionCollection = null;
                this.SecureDetails = null;

                if (!Startup.UserScopes.Contains(securityScopes[1]))
                {
                    return this.View("AdminConsent");
                }

                var sdkQueryBuilder = new StringBuilder();
                var restQueryBuilder = new StringBuilder();

                sdkQueryBuilder.Append($"await graphClient.Security.Alerts[\"{updateAlertModel.AlertId}\"].Request().UpdateAsync(updatedAlert)");

                restQueryBuilder.Append($"PATCH <a>https://graph.microsoft.com/{graphUrlVersion}/security/alerts/{updateAlertModel.AlertId}</a>");

                var email = await this.graphService.GetMyEmailAddress();
                if (!string.IsNullOrEmpty(email))
                {
                    email = $" alert.AssignedTo = {email}; ";
                }

                restQueryBuilder.Append($" Request Body: alert.Status = {updateAlertModel?.NewStatus}; {email} alert.Feedback = {updateAlertModel?.NewFeedback}; alert.Comments = {updateAlertModel?.NewComments} ");

                var updateAlertResultModel = new UpdateAlertResultModel();

                // Save queries to session
                this.ResultQueries = new ResultQueriesViewModel
                {
                    SDKQuery = sdkQueryBuilder.ToString(),
                    RESTQuery = restQueryBuilder.ToString()
                };

                if (string.IsNullOrEmpty(updateAlertModel.AlertId))
                {
                    updateAlertResultModel.Error = "Please enter valid Alert Id";
                    this.UpdateAlertResult = updateAlertResultModel;

                    return this.View("Graph");
                }

                Alert alert = await this.graphService.GetAlertById(updateAlertModel.AlertId);
                if (alert == null)
                {
                    updateAlertResultModel.Error = $"No alert matching this ID {updateAlertModel.AlertId} was found";
                    this.UpdateAlertResult = updateAlertResultModel;

                    return this.View("Graph");
                }
                var commentsSbBefore = new StringBuilder();
                foreach (var comment in alert?.Comments.ToList())
                {
                    commentsSbBefore.Append(comment);
                    commentsSbBefore.Append("<br />");
                }

                updateAlertResultModel.Before = new UpdateAlertResultItemModel
                {
                    Title = alert.Title,
                    Status = alert.Status.ToString(),
                    Comments = commentsSbBefore.ToString(),
                    Feedback = alert.Feedback.ToString(),
                    AssignedTo = alert.AssignedTo,
                    Category = alert.Category,
                    Provider = alert.VendorInformation.Provider,
                    Severity = alert.Severity.ToString()
                };

                await this.graphService.UpdateAlert(alert, updateAlertModel);
                Alert alertUpdated = await this.graphService.GetAlertById(alert.Id);

                var commentsSbAfter = new StringBuilder();
                foreach (var comment in alertUpdated.Comments.ToList())
                {
                    commentsSbAfter.Append(comment);
                    commentsSbAfter.Append("<br />");
                }
                updateAlertResultModel.After = new UpdateAlertResultItemModel
                {
                    Title = alertUpdated.Title,
                    Status = alertUpdated.Status.ToString(),
                    Comments = commentsSbAfter.ToString(),
                    Feedback = alertUpdated.Feedback.ToString(),
                    AssignedTo = alertUpdated.AssignedTo,
                    Category = alertUpdated.Category,
                    Provider = alertUpdated.VendorInformation.Provider,
                    Severity = alertUpdated.Severity.ToString()
                };

                // Update current alert
                this.CurrentAlert.Status = alertUpdated.Status?.ToString() ?? string.Empty;
                this.CurrentAlert.Feedback = alertUpdated.Feedback?.ToString() ?? string.Empty;
                this.CurrentAlert.Comments = alertUpdated.Comments;

                this.UpdateAlertResult = updateAlertResultModel;

                return this.View("Graph");
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded)
                {
                    return new EmptyResult();
                }

                return this.RedirectToAction("Index", "Error", new { message = Resource.Error_Message + Request.RawUrl + ": " + se.Error.Message });
            }
            catch (Exception ex)
            {
                return this.RedirectToAction("Index", "Error", new { message = Resource.Error_Message + Request.RawUrl + ": " + ex.Message });
            }
        }

        /// <summary>
        /// Gets the device details which helps in further investigation of alert
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> GetDevice(string id)
        {
            if (this.CurrentAlert?.User != null)
            {
                this.CurrentAlert.User.SelectedDevice = await this.graphService.GetDeviceById(id);
            }

            return this.View("Graph");
        }

        /// <summary>
        /// Gets the alert by alert id
        /// </summary>
        /// <param name="id">Id of the alert</param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> GetAlert(string id)
        {
            this.SubscriptionCollection = null;
            this.UpdateAlertResult = null;
            this.SecureDetails = null;
            this.AlertFilters = new AlertFilterViewModel { Top = 1, Filters = new AlertFilterCollection() };

            try
            {
                await this.CheckProviderList();
                var alert = await this.graphService.GetAlertById(id);

                if (alert == null)
                {
                    this.CurrentAlert = null;
                    return this.View("Graph");
                }

                var sdkQueryBuilder = new StringBuilder();
                var restQueryBuilder = new StringBuilder();
                sdkQueryBuilder.Append($"await graphClient.Security.Alerts[\"{id}\"].Request().GetAsync()");

                restQueryBuilder.Append($"<a href=\"https://developer.microsoft.com/en-us/graph/graph-explorer?request=security/alerts/{id}&method=GET&version={graphUrlVersion}&GraphUrl=https://graph.microsoft.com\" target=\"_blank\">https://graph.microsoft.com/{graphUrlVersion}/security/alerts/{id}/</a>");

                var alertModel = new AlertDetailsViewModel
                {
                    Id = alert.Id,
                    Title = alert.Title,
                    Description = alert.Description,
                    CreatedDateTime = alert.CreatedDateTime,
                    Status = alert.Status?.ToString() ?? string.Empty,
                    Severity = alert.Severity.ToString(),
                    Feedback = alert.Feedback?.ToString() ?? string.Empty,
                    Vendor = alert.VendorInformation,
                    RecommendedActions = alert.RecommendedActions,
                    Comments = alert.Comments?.ToList(),
                    Triggers = alert.Triggers,
                    Users = alert.UserStates,
                    Hosts = alert.HostStates,
                    NetworkConnections = alert.NetworkConnections,
                    Files = alert.FileStates,
                    Processes = alert.Processes,
                    RegistryKeyUpdates = alert.RegistryKeyStates,
                    CloudAppStates = alert.CloudAppStates,
                    DetectionIds = alert.DetectionIds,
                    MalwareStates = alert.MalwareStates,
                    SourceMaterials = alert.SourceMaterials,
                    Tags = alert.Tags,
                    VulnerabilityStates = alert.VulnerabilityStates
                };

                // Get info about user
                var principalName = alert.UserStates?.FirstOrDefault()?.UserPrincipalName;
                if (!string.IsNullOrWhiteSpace(principalName))
                {
                    alertModel.User = await this.graphService.GetUserDetails(principalName, populatePicture: true, populateManager: true, populateDevices: true);
                   
                    // Add upn
                    if (alertModel.User != null)
                    {
                        alertModel.User.Upn = principalName;
                    }
                }

                // Get info about user alert assigned to
                if (!string.IsNullOrWhiteSpace(alert.AssignedTo))
                {
                    alertModel.AssignedTo = await this.graphService.GetUserDetails(alert.AssignedTo, populatePicture: false, populateManager: false, populateDevices: false);
                    
                    // Add upn
                    if (alertModel.AssignedTo != null)
                    {
                        alertModel.AssignedTo.Upn = alert.AssignedTo;
                    }
                }

                // Get info about host device
                var hostState = alert.HostStates?.FirstOrDefault();
                if (hostState != null)
                {
                    alertModel.Device = new AlertDeviceViewModel
                    {
                        Fqdn = hostState.Fqdn,
                        IsAzureDomainJoined = hostState.IsAzureAdJoined,
                        PublicIpAddress = hostState.PublicIpAddress,
                        PrivateIpAddress = hostState.PrivateIpAddress
                    };
                }

                // Save queries to session
                this.ResultQueries = new ResultQueriesViewModel
                {
                    SDKQuery = sdkQueryBuilder.ToString(),
                    RESTQuery = restQueryBuilder.ToString()
                };

                this.CurrentAlert = alertModel;

                return this.View("Graph");
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded)
                {
                    return new EmptyResult();
                }

                return this.RedirectToAction("Index", "Error",
                    new { message = Resource.Error_Message + Request.RawUrl + ": " + se.Error.Message });
            }
        }

        /// <summary>
        /// Gets subscriptions
        /// </summary>
        /// <returns>List of subscriptions</returns>
        [Authorize]
        public async Task<ActionResult> GetSubscriptions()
        {
            await this.CheckProviderList();
            this.CurrentAlert = null;
            this.AlertSearchResult = null;
            this.UpdateAlertResult = null;
            this.SecureDetails = null;
            this.CreateSubscriptionResult = null;

            try
            {
                IGraphServiceSubscriptionsCollectionPage subscriptions = await this.graphService.ListSubscriptions();

                var sdkQueryBuilder = new StringBuilder();
                var restQueryBuilder = new StringBuilder();
                sdkQueryBuilder.Append("graphClient.Subscriptions.Request().GetAsync()");

                restQueryBuilder.Append($"<a href=\"https://developer.microsoft.com/en-us/graph/graph-explorer?request=subscriptions&&method=GET&version={graphUrlVersion}&GraphUrl=https://graph.microsoft.com\" target=\"_blank\">https://graph.microsoft.com/{graphUrlVersion}/subscriptions</a>");

                this.SubscriptionCollection = subscriptions?.Select(sa => new SubscriptionResultModel
                {
                    Id = sa.Id,
                    Resource = sa.Resource,
                    ExpirationDateTime = sa.ExpirationDateTime,
                    ClientState = sa.ClientState,
                    NotificationUrl = sa.NotificationUrl
                }) ?? Enumerable.Empty<SubscriptionResultModel>();

                // Save queries to session
                this.ResultQueries = new ResultQueriesViewModel
                {
                    SDKQuery = sdkQueryBuilder.ToString(),
                    RESTQuery = restQueryBuilder.ToString()
                };

                return this.View("Graph");
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded)
                {
                    return new EmptyResult();
                }

                return this.RedirectToAction("Index", "Error", new { message = Resource.Error_Message + Request.RawUrl + ": " + se.Error.Message });
            }
            catch (Exception ex)
            {
                return this.RedirectToAction("Index", "Error", new { message = Resource.Error_Message + Request.RawUrl + ": " + ex.Message });
            }
        }

        /// <summary>
        /// Gets secure scores
        /// </summary>
        /// <returns>List of secure scores</returns>
        [Authorize]
        public async Task<ActionResult> GetSecureDetails()
        {
            await this.CheckProviderList();
            this.AlertFilters = new AlertFilterViewModel { Top = 1, Filters = new AlertFilterCollection() };
            this.CurrentAlert = null;
            this.AlertSearchResult = null;
            this.UpdateAlertResult = null;
            this.SubscriptionCollection = null;

            try
            {
                string accessToken = await SampleAuthProvider.Instance.GetUserAccessTokenAsync();
                //// Get top secure score
                var secureScores = await this.graphService.GetSecureScore(accessToken, "?$top=1");

                if(secureScores == null || secureScores.Count == 0)
                {
                    return this.RedirectToAction("Index", "Error", "Cannot get securescore for the tenant. This tenant might not have secure score data or the user might not have proper permissions");
                }

                var topSecureScore = secureScores.FirstOrDefault();

                //// Get control profiles
                var controlProfiles = await this.graphService.GetSecureScoreControlProfiles(accessToken);
        
                var restQueryBuilder = new StringBuilder();           
                restQueryBuilder.Append($"<a href=\"https://developer.microsoft.com/en-us/graph/graph-explorer?request=security/securescores?$top=1&&method=GET&version=beta&GraphUrl=https://graph.microsoft.com\" target=\"_blank\">https://graph.microsoft.com/beta/security/securescores?$top=1</a>");

                this.SecureDetails = new SecureScoreDetailsViewModel
                {
                    TopSecureScore = new SecureScoreModel
                    {
                        CreatedDateTime = topSecureScore.CreatedDateTime,
                        CurrentScore = topSecureScore.CurrentScore,
                        MaxScore = topSecureScore.MaxScore,
                        EnabledServices = topSecureScore.EnabledServices.Select(service => service.StartsWith("Has", StringComparison.CurrentCultureIgnoreCase) ? service.Substring(3) : service),
                        LicensedUserCount = topSecureScore.LicensedUserCount.HasValue ? topSecureScore.LicensedUserCount.Value : 0,
                        ActiveUserCount = topSecureScore.ActiveUserCount.HasValue ? topSecureScore.ActiveUserCount.Value : 0,
                        Id = topSecureScore.Id,
                        ControlScores = topSecureScore.ControlScores?.Select(controlScore => new ControlScoreModel
                        {
                            ControlCategory = controlScore.ControlCategory,
                            Score = controlScore.Score.HasValue ? controlScore.Score.Value : 0.0,
                            Name = controlScore.ControlName,
                            Description = controlScore.Description,
                            Count = controlScore.AdditionalData.ContainsKey("count")
                                ? controlScore.AdditionalData["count"].ToString()
                                : controlScore.AdditionalData.ContainsKey("on")
                                ? controlScore.AdditionalData["on"].ToString()
                                : "0.0",
                            Total = controlScore.AdditionalData.ContainsKey("total")
                                ? controlScore.AdditionalData["total"].ToString()
                                : controlScore.AdditionalData.ContainsKey("IntuneOn")
                                ? controlScore.AdditionalData["IntuneOn"].ToString()
                                : "0.0",
                        }) ?? Enumerable.Empty<ControlScoreModel>(),
                        AverageComparativeScores = topSecureScore.AverageComparativeScores?.Select(score =>
                        {
                            string Value = string.Empty;
                            switch (score.Basis)
                            {
                                case "Industry":
                                    {
                                        if (score.AdditionalData.ContainsKey("industryName"))
                                        {
                                            Value = score.AdditionalData["industryName"].ToString();
                                        }
                                    }
                                    break;
                                case "SeatCategory":
                                    {
                                        if (score.AdditionalData.ContainsKey("SeatLowerLimit") && score.AdditionalData.ContainsKey("SeatUpperLimit"))
                                        {
                                            Value = $"{score.AdditionalData["SeatLowerLimit"]} - {score.AdditionalData["SeatUpperLimit"]}";
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        Value = string.Join(", ", score.AdditionalData.Select(item => item.Value));
                                    }
                                    break;
                            }

                            return new AverageComparativeScoreModel
                            {
                                ComparedBy = score.Basis,
                                AverageScore = score.AverageScore.HasValue ? score.AverageScore.Value : 0.0,
                                Value = Value
                            };
                        }) ?? Enumerable.Empty<AverageComparativeScoreModel>()
                    },
                  SecureScoreControlProfiles = controlProfiles?.Select(profile => profile) ?? Enumerable.Empty<SecureScoreControlProfileModel>()
                };

                // Save queries to session
                this.ResultQueries = new ResultQueriesViewModel
                {
                    RESTQuery = restQueryBuilder.ToString()
                };

                return this.View("Graph");
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded)
                {
                    return new EmptyResult();
                }

                return this.RedirectToAction("Index", "Error", new { message = Resource.Error_Message + Request.RawUrl + ": " + se.Error.Message });
            }
            catch (Exception ex)
            {
                return this.RedirectToAction("Index", "Error", new { message = Resource.Error_Message + Request.RawUrl + ": " + ex.Message });
            }
        }

        /// <summary>
        ///  Get the alerts based on single filter
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> Filter(string key, string value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                {
                    return this.RedirectToAction("Index", "Error", new { message = Resource.Error_Message + "key and value params can't be empty." });
                }

                await this.CheckProviderList();
                this.CurrentAlert = null;
                this.UpdateAlertResult = null;
                this.SubscriptionCollection = null;
                this.SecureDetails = null;

                var viewAlertFilter = new AlertFilterViewModel { Top = 50, Filters = new AlertFilterCollection() };
                viewAlertFilter.Filters.Add(key, value);

                var orderByOarams = new Dictionary<string, string>();

                switch (key)
                {
                    case "alert:severity":
                        {
                            orderByOarams.Add("createdDateTime", "desc");
                        }
                        break;
                    default:
                        {
                            orderByOarams.Add("severity", "desc");
                            orderByOarams.Add("createdDateTime", "desc");
                        }
                        break;
                }

                var filter = new AlertFilterModel(viewAlertFilter);
                var securityAlertsResult = await this.graphService.GetAlerts(filter, orderByOarams);
                var filterQuery = securityAlertsResult?.Item2 ?? string.Empty;

                // Generate queries
                var sdkQueryBuilder = new StringBuilder();
                var restQueryBuilder = new StringBuilder();
                sdkQueryBuilder.Append("await graphClient.Security.Alerts.Request()");
                if (!string.IsNullOrEmpty(filterQuery))
                {
                    sdkQueryBuilder.Append($".Filter(\"{filterQuery}\")");
                }
                sdkQueryBuilder.Append($".Top({filter.Top}).GetAsync()");

                if (!string.IsNullOrEmpty(filterQuery))
                {
                    restQueryBuilder.Append($"<a href=\"https://developer.microsoft.com/en-us/graph/graph-explorer?request=security/alerts?$filter={HttpUtility.UrlEncode(filterQuery)}%26$top={filter.Top}&&method=GET&version={this.graphUrlVersion}&GraphUrl=https://graph.microsoft.com\" target=\"_blank\">https://graph.microsoft.com/{this.graphUrlVersion}/security/alerts?");

                    restQueryBuilder.Append($"$filter={HttpUtility.UrlEncode(filterQuery)}&");
                    restQueryBuilder.Append($"$top={filter.Top}</a>");
                }
                else
                {
                    restQueryBuilder.Append($"<a href=\"https://developer.microsoft.com/en-us/graph/graph-explorer?request=security/alerts?$top={filter.Top}&&method=GET&version={this.graphUrlVersion}&GraphUrl=https://graph.microsoft.com\" target=\"_blank\">https://graph.microsoft.com/{this.graphUrlVersion}/security/alerts?");
                    restQueryBuilder.Append($"$top={filter.Top}</a>");
                }

                // Save alerts to session
                this.AlertSearchResult = securityAlertsResult?.Item1?.Select(sa => new AlertResultItemModel
                {
                    Id = sa.Id,
                    Title = sa.Title,
                    Status = sa.Status,
                    Provider = sa.VendorInformation?.Provider,
                    CreatedDateTime = sa.CreatedDateTime,
                    Severity = sa.Severity.ToString(),
                    Category = sa.Category
                }) ?? Enumerable.Empty<AlertResultItemModel>();

                // Save queries to session
                this.ResultQueries = new ResultQueriesViewModel
                {
                    SDKQuery = sdkQueryBuilder.ToString(),
                    RESTQuery = restQueryBuilder.ToString()
                };

                this.AlertFilters = viewAlertFilter;

                return this.View("Graph");
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded)
                {
                    return new EmptyResult();
                }

                return this.RedirectToAction("Index", "Error", new { message = Resource.Error_Message + Request.RawUrl + ": " + se.Error.Message });
            }
        }

        public ActionResult About()
        {
            return this.View();
        }
    }
}