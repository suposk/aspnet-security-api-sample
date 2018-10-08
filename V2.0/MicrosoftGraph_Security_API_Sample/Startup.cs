// -----------------------------------------------------------------------
// <copyright file="Startup.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Owin;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(MicrosoftGraph_Security_API_Sample.Startup))]

namespace MicrosoftGraph_Security_API_Sample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();

            this.ConfigureAuth(app);
        }
    }
}
