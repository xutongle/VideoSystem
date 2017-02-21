using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoSystem.Models.Concrete;

namespace VideoSystem.Controllers.Front
{
    public class PhoneCodeController : Controller
    {
        //
        // GET: /PhoneCode/

        public ActionResult Index(string phone)
        {
            string result = PhoneCode.getPhoneCode(phone);

            return Content(result);
        }
    }
}
