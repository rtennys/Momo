using System;
using System.Configuration;
using DotNetOpenAuth.AspNet.Clients;
using Microsoft.Web.WebPages.OAuth;

namespace Momo.UI
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterTwitterClient(
            //    consumerKey: "",
            //    consumerSecret: "");

            OAuthWebSecurity.RegisterFacebookClient(ConfigurationManager.AppSettings["FacebookAppId"], ConfigurationManager.AppSettings["FacebookAppSecret"]);
            OAuthWebSecurity.RegisterMicrosoftClient(ConfigurationManager.AppSettings["MicrosoftClientId"], ConfigurationManager.AppSettings["MicrosoftClientSecret"]);
            OAuthWebSecurity.RegisterGoogleClient();
            OAuthWebSecurity.RegisterClient(new OpenIdClient("MyOpenId", "https://myopenid.com/"), "MyOpenID", null);
            OAuthWebSecurity.RegisterYahooClient();
        }
    }
}
