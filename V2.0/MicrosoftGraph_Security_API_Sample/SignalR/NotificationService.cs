// -----------------------------------------------------------------------
// <copyright file="NotificationService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using MicrosoftGraph_Security_API_Sample.Models;

namespace MicrosoftGraph_Security_API_Sample.SignalR
{
    public class NotificationService : PersistentConnection
    {
        public void SendNotificationToClient(List<NotificationViewModel> messages)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            if (hubContext != null)
            {
                hubContext.Clients.All.showNotification(messages);
            }
        }
    }
}