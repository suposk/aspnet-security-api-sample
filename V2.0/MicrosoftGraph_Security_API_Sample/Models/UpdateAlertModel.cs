// -----------------------------------------------------------------------
// <copyright file="UpdateAlertModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MicrosoftGraph_Security_API_Sample.Models
{
    public class UpdateAlertModel
    {
        public string AlertId { get; set; }

        public string NewStatus { get; set; }

        public string NewFeedback { get; set; }

        public string NewComments { get; set; }
    }
}