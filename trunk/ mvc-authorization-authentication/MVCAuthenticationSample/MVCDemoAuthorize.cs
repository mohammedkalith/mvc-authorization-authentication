using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Authentication;
using System.Collections;
using System.Text;
using DataAccess;

namespace MVCAuthenticationSample
{
    public class MVCDemoAuthorizeAttribute : AuthorizeAttribute
    {
    
        public Modules Module { get; set; }
        public Functions Rights { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            var principal = filterContext.HttpContext.User as AuthenticationProjectPrincipal;
            if (principal == null)
                CloseConnection(filterContext);

            var isAuthorized = false;
            var moduleId = (byte)Module;
            ArrayList functionIds = new ArrayList();

            HttpSessionStateBase session = filterContext.HttpContext.Session;
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendFormat("{0}_{1}_", principal.UserData.UserID, moduleId);

            foreach (Functions right in Enum.GetValues(typeof(Functions)))
            {
                if (!((Convert.ToInt32(Rights) & Convert.ToInt32(right)) == Convert.ToInt32(right))) continue;
            
                functionIds.Add(right);
                strBuilder.Append((byte)right);
            }

            string cacheKey = strBuilder.ToString();
            if (session[cacheKey] != null)
            {
                isAuthorized = (bool)session[cacheKey];
                if (isAuthorized)
                    return;

                CloseConnection(filterContext);
            }

            isAuthorized = this.IsUserAuthorized(principal.UserData.UserID, moduleId, Array.ConvertAll(functionIds.ToArray(), arr => (int)arr));
           
            session.Add(cacheKey, isAuthorized);

            if (!isAuthorized)
                CloseConnection(filterContext);
        }

        /// <summary>
        /// Closes the connection and sets the status code to Unauthorized
        /// </summary>
        /// <param name="filterContext">The AuthorizationContext</param>
        private void CloseConnection(AuthorizationContext filterContext)
        {
            HttpResponseBase response = filterContext.HttpContext.Response;

            response.StatusCode = 401;//Unauthorized status code
            response.Write("You are not authorized!");
            response.Flush();
            response.Close();
            response.End();
        }

        #region helper methods
        public bool IsUserAuthorized(int userId, byte moduleId, int[] functionIds)
        {
            using (AuthenticationDemoEntities dbContext = new AuthenticationDemoEntities())
            {
                try
                {
                    var user = dbContext.Users
                        .Include("AccessToModuleFunctions")
                        .Include("Role")
                        .Include("AccessToModuleFunctions.ModulesFunctions")
                        .Include("AccessToModuleFunctions.ModulesFunctions.Module")
                        .Include("AccessToModuleFunctions.ModulesFunctions.Function")
                        .Include("Role.AccessToModuleFunctions")
                        .Include("Role.AccessToModuleFunctions.ModulesFunctions")
                        .Include("Role.AccessToModuleFunctions.ModulesFunctions.Module")
                        .Include("Role.AccessToModuleFunctions.ModulesFunctions.Function")
                        .FirstOrDefault(usr => usr.ID == userId);

                    if (user == null)
                        return false;

                    IEnumerable<AccessToModuleFunctions> userModFuncs = user.AccessToModuleFunctions
                            .Where(modFunc => modFunc.ModulesFunctions.Module.ID == moduleId
                                && functionIds.Contains((int)modFunc.ModulesFunctions.Function.ID));

                    if (userModFuncs.Count() == 0)
                        userModFuncs = user.Role.AccessToModuleFunctions
                            .Where(modFunc => modFunc.ModulesFunctions.Module.ID == moduleId
                                && functionIds.Contains((int)modFunc.ModulesFunctions.Function.ID));

                    if (userModFuncs.Count() == 0)
                        return false;

                    bool hasAccess = true;
                    foreach (var usrModFunc in userModFuncs)
                    {
                        hasAccess = usrModFunc.HasAccess ?? false;
                        if (!hasAccess)
                            break;
                    }

                    return hasAccess;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        #endregion
    }
}
