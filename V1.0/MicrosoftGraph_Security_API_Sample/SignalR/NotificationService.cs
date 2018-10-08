/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

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