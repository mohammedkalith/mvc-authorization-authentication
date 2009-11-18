using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Authentication;

namespace MVCAuthenticationSample
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Authenticate", action = "Index", id = "" }  // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);
        }


        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            string cookie = FormsAuthentication.FormsCookieName;
            HttpCookie httpCookie = Context.Request.Cookies[cookie];

            if (httpCookie == null) return;

            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(httpCookie.Value);
            if (ticket == null || ticket.Expired) return;

            FormsIdentity identity = new FormsIdentity(ticket);
            UserData udata = UserData.CreateUserData(ticket.UserData);
            AuthenticationProjectPrincipal principal = new AuthenticationProjectPrincipal(identity, udata);
            Context.User = principal;
        }

    }
}