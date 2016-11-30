using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoSystem.Filters;

namespace VideoSystem.Controllers.Back
{
    [CustAuthorize("admin")]
    public class LimitsManagerController : Controller
    {
        //
        // GET: /LimitsManager/
        public ActionResult Index()
        {
            
            return View();
        }
        

        public ActionResult AddManagerPage()
        {
            return View();
        }

    }
}
