// -----------------------------------------------------------------------
// <copyright file="MultiButtonAttribute.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reflection;
using System.Web.Mvc;

namespace MicrosoftGraph_Security_API_Sample.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MultiButtonAttribute : ActionNameSelectorAttribute
    {
        public string MatchFormKey { get; set; }

        public string MatchFormValue { get; set; }

        public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
        {
            return controllerContext.HttpContext.Request[this.MatchFormKey] != null &&
                controllerContext.HttpContext.Request[this.MatchFormKey] == this.MatchFormValue;
        }
    }
}