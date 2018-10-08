/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using Microsoft.Graph;

namespace MicrosoftGraph_Security_API_Sample.Models
{
    public class AlertResultItemModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public AlertStatus? Status { get; set; }
        public string AssignedTo { get; set; }
        public string Provider { get; set; }
    }
}