/*
 *  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
 *  See LICENSE in the source repository root for complete license information.
 */

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MicrosoftGraph_Security_API_Sample.Models;
using MicrosoftGraph_Security_API_Sample.SignalR;
using MicrosoftGraph_Security_API_Sample.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Security.Claims;

namespace MicrosoftGraph_Security_API_Sample.Controllers
{
    using MicrosoftGraph_Security_API_Sample.SignalR;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;

    public class NotificationController : Controller
    {
        public ActionResult LoadView()
        {
            ViewBag.CurrentUserId = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            return View("Notification");
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
            if (Request.QueryString["validationToken"] != null)
            {
                var token = Request.QueryString["validationToken"];
                return Content(token, "plain/text");
            }

            // Parse the received notifications.
            else
            {
                
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
                        Notification notifcation = notifications.Value.FirstOrDefault();
                        notifcation.EntityId = notifcation.ResourceData.Id;
                        List<NotificationViewModel> messages = new List<NotificationViewModel>();
                        NotificationViewModel messageViewModel = new NotificationViewModel(notifcation, notifcation.SubscriptionId);
                        messages.Add(messageViewModel);

                        NotificationService notificationService = new NotificationService();
                        notificationService.SendNotificationToClient(messages);

                    }
                
                    }
                catch (Exception)
                {

                    // TODO: Handle the exception.
                    // Still return a 202 so the service doesn't resend the notification.
                }



                return new HttpStatusCodeResult(202);
            }
        }  
    }
}
