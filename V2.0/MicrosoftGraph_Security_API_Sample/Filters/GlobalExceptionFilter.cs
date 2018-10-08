// -----------------------------------------------------------------------
// <copyright file="GlobalExceptionFilter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Web.Mvc;
using Microsoft.Graph;

namespace MicrosoftGraph_Security_API_Sample.Filters
{
    public class GlobalUnauthorizeExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext exceptionContext)
        {
            if (!exceptionContext.ExceptionHandled && exceptionContext.Exception is ServiceException)
            {
                var exception = (ServiceException)exceptionContext.Exception;
                if (exception is ServiceException && exception.Error.Code.Equals("AuthenticationFailure", StringComparison.OrdinalIgnoreCase))
                {
                    // Redirect to login page
                    exceptionContext.Result = new HttpUnauthorizedResult();
                    exceptionContext.ExceptionHandled = true;
                }
            }
        }
    }
}
