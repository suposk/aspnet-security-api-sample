// -----------------------------------------------------------------------
// <copyright file="ErrorController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Web.Mvc;

namespace MicrosoftGraph_Security_API_Sample.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index(string message)
        {
            this.ViewBag.Message = message;
            return this.View("Error");
        }
    }
}