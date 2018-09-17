using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.Mobile.Server.Login;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using TestCustomAuth.DataObjects;

namespace TestCustomAuth.Controllers
{
    [MobileAppController]
    public class AuthController : ApiController
    {
        public HttpResponseMessage Post([FromBody]LoginChallenge challenge)
        {
            // return error if password is not correct
            if (!this.IsPasswordValid(challenge.Username, challenge.Password))
            {
                return this.Request.CreateUnauthorizedResponse();
            }

            JwtSecurityToken token = this.GetAuthenticationTokenForUser(challenge.Username);

            return this.Request.CreateResponse(HttpStatusCode.OK, new
            {
                Token = token.RawData,
                Username = challenge.Username
            });
        }

        private JwtSecurityToken GetAuthenticationTokenForUser(string username)
        {
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username)
            };

            var signingKey = this.GetSigningKey();
            var audience = this.GetSiteUrl(); // audience must match the url of the site
            var issuer = this.GetSiteUrl(); // audience must match the url of the site 

            JwtSecurityToken token = AppServiceLoginHandler.CreateToken(
                claims,
                signingKey,
                audience,
                issuer,
                TimeSpan.FromHours(24)
                );

            return token;
        }

        private bool IsPasswordValid(string username, string password)
        {
            // this is where we would do checks agains a database

            return true;
        }

        private string GetSiteUrl()
        {
            var settings = this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

            if (string.IsNullOrEmpty(settings.HostName))
            {
                return ConfigurationManager.AppSettings["ValidAudience"];
            }
            else
            {
                return "https://" + settings.HostName + "/";
            }
        }

        private string GetSigningKey()
        {
            var settings = this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

            if (string.IsNullOrEmpty(settings.HostName))
            {
                // this key is for debuggint and testing purposes only
                // this key should match the one supplied in Startup.MobileApp.cs
                return ConfigurationManager.AppSettings["SigningKey"];
            }
            else
            {
                return Environment.GetEnvironmentVariable("WEBSITE_AUTH_SIGNING_KEY");
            }
        }
    }
}
