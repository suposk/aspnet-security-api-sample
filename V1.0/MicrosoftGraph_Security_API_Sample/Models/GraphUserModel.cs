/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using Microsoft.Graph;
using System.Collections.Generic;
using System.IO;

namespace MicrosoftGraph_Security_API_Sample.Models
{
    public class GraphUserModel
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string JobTitle { get; set; }
        public GraphUserModel Manager { get; set; }     
        public string OfficeLocation { get; set; }
        public string ContactVia { get; set; }

        public string Picture { get; set; }
        public IEnumerable<GraphDeviceModel> RegisteredDevices { get; set; }
        public IEnumerable<GraphDeviceModel> OwnedDevices { get; set; }

        public GraphDeviceModel SelectedDevice { get; set; }
    }
}