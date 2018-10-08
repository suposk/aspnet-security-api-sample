// -----------------------------------------------------------------------
// <copyright file="FilterConfig.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Web.Mvc;
using MicrosoftGraph_Security_API_Sample.Filters;

namespace MicrosoftGraph_Security_API_Sample
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new GlobalUnauthorizeExceptionFilter());
        }
    }
}
