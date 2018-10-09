/*
 *  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
 *  See LICENSE in the source repository root for complete license information.
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Security.Claims;
using MicrosoftGraph_Security_API_Sample.Models;
using MicrosoftGraph_Security_API_Sample.SignalR;
using MicrosoftGraph_Security_API_Sample.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MicrosoftGraph_Security_API_Sample.Controllers
{
    public class NotificationController : Controller
    {
        public ActionResult Index()
        {
            this.ViewBag.CurrentUserId = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            return this.View("Notification");
        }

        /// <summary>
        /// Listener for the notifications 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Listen()
        {
            // Validate the new subscription by sending the token back to Microsoft Graph.
            // This response is required for each subscription.
            // Parse the received notifications.
            if (Request.QueryString["validationToken"] != null)
            {
                var token = Request.QueryString["validationToken"];
                return this.Content(token, "plain/text");
            }
            else
            {
                NotificationService notificationService = new NotificationService();
                List<NotificationViewModel> messages = new List<NotificationViewModel>();

                try
                {
                    string documentContents;
                    using (var inputStream = Request.InputStream)
                    {
                        using (StreamReader readStream = new StreamReader(inputStream, Encoding.UTF8))
                        {
                            documentContents = readStream.ReadToEnd();
                        }

                        var notifications = JsonConvert.DeserializeObject<NotificationCollection>(documentContents);
                        foreach (Notification notification in notifications.Value)
                        {
                            notification.EntityId = notification.ResourceData.Id;
                            NotificationViewModel messageViewModel = new NotificationViewModel(notification, notification.SubscriptionId);
                            messages.Add(messageViewModel);
                        }
                        if (messages.Count > 0)
                        {
                            notificationService.SendNotificationToClient(messages);
                        }
                    }
                }
                catch (Exception)
                {
                    // TODO: Handle the exception.
                    // Still return a 202 so the service doesn't resend the notification.
                }

                return await Task.FromResult<ActionResult>(new HttpStatusCodeResult(202));
            }
        }
    }
}
