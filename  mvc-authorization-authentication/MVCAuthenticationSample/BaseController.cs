using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Authentication;
using DataAccess;

namespace MVCAuthenticationSample
{
    public class BaseController : Controller
    {
        private const string cookieName = "AccessibleMenus";

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            HttpCookie cookie = requestContext.HttpContext.Request.Cookies[cookieName];
            if (cookie != null)
            {
                ViewData[cookieName] = cookie.Value.Split(',').ToList();
                return;
            }

            AuthenticationProjectPrincipal raPrincipal = requestContext.HttpContext.User as AuthenticationProjectPrincipal;
            if (raPrincipal == null)
                return;

            IList<string> modules = this.GetAccessableModuleNames(raPrincipal.UserData.UserID);
            ViewData[cookieName] = modules;

            cookie = new HttpCookie(cookieName, string.Join(",", modules.ToArray()));
            cookie.Expires = DateTime.Now.AddMinutes(20);
            requestContext.HttpContext.Response.Cookies.Add(cookie);
        }


        #region helper methods

        [NonAction()]
        private IList<string> GetAccessableModuleNames(int userId)
        {
            try
            {
                IList<string> modNames = new List<string>();
                foreach (var module in Enum.GetValues(typeof(Modules)))
                {
                    if (this.HasUserAccessToModule(userId, Convert.ToByte(module)))
                        modNames.Add(module.ToString());
                }

                return modNames;
            }
           
            catch (Exception e)
            {
                throw e;
            }
        }
        [NonAction()]
        private bool HasUserAccessToModule(int userId, byte moduleId)
        {
            using (AuthenticationDemoEntities dbContext = new AuthenticationDemoEntities())
            {
                try
                {
                    int functionId = Convert.ToInt32(Functions.Access);
                 
                    Users user = dbContext.Users
                        .Include("AccessToModuleFunctions")
                        .Include("Role")
                        .Include("AccessToModuleFunctions.ModulesFunctions")
                        .Include("AccessToModuleFunctions.ModulesFunctions.Module")
                        .Include("AccessToModuleFunctions.ModulesFunctions.Function")
                        .FirstOrDefault(usr => usr.ID == userId);

                    if (user == null)
                        return false;

                       AccessToModuleFunctions userFunc = user.AccessToModuleFunctions
                        .SingleOrDefault(modFunc => modFunc.ModulesFunctions.Function.ID == functionId
                            && modFunc.ModulesFunctions.Module.ID == moduleId);
                    if (userFunc == null)
                    {
                        if (user.Role == null)
                            return false;

                        userFunc = GetAccessByRoleModuleFunciton(user.Role.ID, moduleId, (long)Functions.Access);
                        if (userFunc == null)
                            return false;
                    }

                    return userFunc.HasAccess ?? false;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        private AccessToModuleFunctions GetAccessByRoleModuleFunciton(int roleId, byte moduleId, long functionId)
        {
            using (AuthenticationDemoEntities dbContext = new AuthenticationDemoEntities())
            {
                try
                {
                    return dbContext.AccessToModuleFunctions
                        .FirstOrDefault(urmf => urmf.Role.ID == roleId && urmf.ModulesFunctions.Module.ID == moduleId
                            && urmf.ModulesFunctions.Function.ID == functionId);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion
    }

   
}
