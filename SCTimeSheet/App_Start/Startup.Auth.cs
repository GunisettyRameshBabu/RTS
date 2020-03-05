using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SCTimeSheet
{
    public partial class Startup
    {

        // Calling the keys values from Web.config file  
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string tenant = ConfigurationManager.AppSettings["ida:Tenant"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
        private static string redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];

        // Concatenate aadInstance, tenant to form authority value       
        private string authority = string.Format(CultureInfo.InvariantCulture, aadInstance, tenant);


        // ConfigureAuth method  
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(

                            new OpenIdConnectAuthenticationOptions
                            {
                                ClientId = clientId,
                                Authority = authority,
                                PostLogoutRedirectUri = postLogoutRedirectUri,
                                RedirectUri = redirectUri,
                                Notifications = new OpenIdConnectAuthenticationNotifications
                                {
                                    AuthenticationFailed = (context) =>
                                    {
                                        context.HandleResponse();
                                        var url = HttpContext.Current.Request.Url;
                                        context.OwinContext.Response.Redirect(url.AbsoluteUri+"/Error/Index");
                                        return Task.FromResult(0);
                                    }
                                }
                            });


        } // end - ConfigureAuth method  

    } // end - Startup class
}