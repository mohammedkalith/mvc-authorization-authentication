using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Security;
using Authentication;
using DataAccess;
using System.Configuration;
namespace MVCAuthenticationSample.Controllers
{
    public class AuthenticateController : BaseController
    {
        
        public ActionResult Index()
        {
            return View();
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(FormCollection collection)
        {
            if (String.IsNullOrEmpty(collection["username"]) || String.IsNullOrEmpty(collection["password"]))
            {
                ViewData["ErrorDetails"] = "The username or password are incorrect.";

                return View();
            }

            var userName = collection["username"];
            var password = collection["password"];

            if (!Authenticate(userName,password))
                return View();

            var logonUser = GetUserByUserName(userName);

            DoLogin(logonUser);
          
            return RedirectToAction("Index", "Home");

        }

        public ActionResult DoLogOff()
        {
            this.ExpireCookie(".ASPXAUTH");
            this.ExpireCookie("AccessibleMenus");
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Authenticate");
        }


        #region Helper methods
        [NonAction()]
        private bool Authenticate(string username, string password)
        {
            ViewData["ErrorDetails"] = string.Empty;
            var fieldAuthenticated = true;
            if (String.IsNullOrEmpty(username))
            {
                fieldAuthenticated = false;

            }
            if (String.IsNullOrEmpty(password))
            {
                fieldAuthenticated = false;

            }

            if (!Login(username, password))
            {
                fieldAuthenticated = false;

            }

            if (!fieldAuthenticated)
            {
                ViewData["ErrorDetails"] = "The username or password provided is incorrect or user is logged in.";

            }

            return fieldAuthenticated;
        }

        [NonAction()]
        private Users GetUserByUserName(string username)
        {
            using (AuthenticationDemoEntities dbContext = new AuthenticationDemoEntities())
            {
                
                var logonUser = dbContext.Users.FirstOrDefault(user => user.UserName.Equals(username));

                if (logonUser == null) return null;

                return logonUser;

            }
        }

        [NonAction()]
        private bool Login(string username, string pass)
        {
            if (username == null)
                throw new ArgumentNullException("userName");

            if (username == string.Empty)
                throw new ArgumentException("userName");

            if (pass == null)
                throw new ArgumentNullException("password");

            if (pass == string.Empty)
                throw new ArgumentException("password");
            
            using (AuthenticationDemoEntities dbContext = new AuthenticationDemoEntities())
            {
                var strDBHash = string.Empty;
                var strDBSalt = string.Empty;
                var logonUser = dbContext.Users.FirstOrDefault(user => user.UserName.Equals( username));

                if (logonUser == null) return false;

                strDBSalt = logonUser.Salt;
                WebSecurity.HashWithSalt(pass, ref strDBSalt, out strDBHash);

                if (strDBHash != logonUser.Hash) return false;
               
            }
            return true;
        }

        [NonAction()]
        private void DoLogin(DataAccess.Users user)
        {

            var timeOut = 400;
            UserData ud = new UserData(user.ID, user.UserName);
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, user.UserName, DateTime.Now, DateTime.Now.AddMinutes(timeOut), false, ud.ToXml());
            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            Response.Cookies.Add(authCookie);
            InvalidateModulesCookie();
        }
        [NonAction()]
        private void InvalidateModulesCookie()
        {
            HttpCookie cookie = HttpContext.Request.Cookies["AccessibleMenus"];
            if (cookie != null)
            {
                cookie.Expires = new DateTime(1970, 1, 1);
                HttpContext.Response.Cookies.Add(cookie);
            }
        }

        [NonAction()]
        private void ExpireCookie(string cookieName)
        {
            if (Request.Cookies[cookieName] != null)
            {
                HttpCookie cookie = new HttpCookie(cookieName);
                cookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(cookie);
            }
        }
        #endregion
    }
}
