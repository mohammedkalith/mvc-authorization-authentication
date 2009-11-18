using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace MVCAuthenticationSample.Controllers
{
    public class AdminController : BaseController
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        [MVCDemoAuthorizeAttribute(Module = Modules.Administration, Rights = Functions.Access | Functions.ManageUsers)]
        //[Authorize(Users="kbochevski,administrator")]
        public ActionResult ModifyUsers()
        {
            return View();
        }

    }
}
