/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

namespace MicrosoftGraph_Security_API_Sample.Models
{
    public class UpdateAlertModel
    {
        public string AlertId { get; set; }
        public string UpdateStatus { get; set; }
        public string Feedback { get; set; }
        public string Comments { get; set; }
    }
}