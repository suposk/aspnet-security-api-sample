/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

namespace MicrosoftGraph_Security_API_Sample.Models
{
    public class AlertFilter
    {
        public string AlertId { get; set; }
        public int? Top { get; set; }
        public string Provider { get; set; }
        public string Category { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }
        public string AssignedToMe { get; set; }
        public string HostFqdn { get; set; }
        public string Upn { get; set; }
        public string Email { get; set; }
        public string FilteredQuery { get; set; }
    }
}