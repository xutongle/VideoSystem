using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VideoSystem.Filters;
using VideoSystem.Models;

namespace VideoSystem.Controllers.Back
{
    public class AdminController : Controller
    {
        private VideoSystemContext vsc = new VideoSystemContext();
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        [CustAuthorize("admin")]
        public ActionResult BackMain()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BackLogin(string account, string password)
        {
            Manager[] managerArray = vsc.Managers.Where(m => m.ManagerAccount == account && m.ManagerPassword == password).ToArray();

            if (managerArray.Count() <= 0)
            {
                TempData["erroInfo"] = "账号或密码错误！";
                return RedirectToAction("", "Admin");
            }
            else
            {
                FormsAuthentication.SetAuthCookie("Manager", false);

                Session["Manager"] = managerArray[0];
                ViewBag.account = managerArray[0].ManagerAccount;
                return RedirectToAction("BackMain", "Admin");
            }
            //FormsAuthentication.SetAuthCookie("Manager", false);
            //return RedirectToAction("BackMain", "Admin");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("","Admin");
        }

    }
}
