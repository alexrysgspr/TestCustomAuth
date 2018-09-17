using Microsoft.Azure.Mobile.Server.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TestCustomAuth.Controllers
{
    [MobileAppController]
    public class ProtectedController : ApiController
    {
        [Authorize]
        public string Get()
        {
            var settings = this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();
            var traceWriter = this.Configuration.Services.GetTraceWriter();

            string host = settings.HostName ?? "localhost";
            string greeting = "Hello from " + host;

            traceWriter.Info(greeting);

            return greeting;
        }
    }
}
