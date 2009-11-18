using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace MVCAuthenticationSample.Controllers
{
    public class InvoiceController : BaseController
    {
        //
        // GET: /Invoice/

        public ActionResult Index()
        {
            return View();
        }

    }
}
