/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using System;
using System.Web;

namespace MicrosoftGraph_Security_API_Sample.Models
{
    public class SubscriptionFilters
    {     
        public string SubscriptionProvider { get; set; }
        public string SubscriptionCategory { get; set; }
        public string SubscriptionSeverity { get; set; }     
        public string SubscriptionHostFqdn { get; set; }
        public string SubscriptionUpn { get; set; }
        public string Email { get; set; }
    }
}
