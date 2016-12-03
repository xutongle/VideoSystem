using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VideoSystem.Filters;
using VideoSystem.Models;

namespace VideoSystem.Controllers.Back
{
    [CustAuthorize("admin")]
    public class SettingsController : Controller
    {
        private VideoSystemContext vsc = new VideoSystemContext();

        //
        // GET: /Settings/

        public ActionResult Index()
        {
            int managerID = Convert.ToInt32(Request.Cookies["abcd"].Value);
            Manager manager = vsc.Managers.Find(managerID);
            ViewBag.account = manager.ManagerAccount;
            return View(manager);
        }

        //编辑信息
        public ActionResult EditInfo(string email,string phone)
        {
            int managerID = Convert.ToInt32(Request.Cookies["abcd"].Value);
            Manager manager = vsc.Managers.Find(managerID);
            manager.ManagerEmail = email;
            manager.ManagerPhone = phone;

            if (ModelState.IsValid)
            {
                vsc.Entry(manager).State = EntityState.Modified;
                vsc.SaveChanges();
            }

            return RedirectToAction("", "Settings");
        }

        //跳转到修改密码页面
        public ActionResult ModifyPassPage() {
            Manager manager = (Manager)Session["Manager"];
            ViewBag.account = manager.ManagerAccount;
            return View();
        }

        //修改密码
        public ActionResult ModifyPass(string oldPass,string newPass)
        {
            int managerID = Convert.ToInt32(Request.Cookies["abcd"].Value);
            Manager manager = vsc.Managers.Find(managerID);

            if (oldPass != manager.ManagerPassword)
            {
                TempData["erroInfo"] = "原始密码不正确";
                return RedirectToAction("ModifyPassPage","Settings");
            }
            else
            {
                manager.ManagerPassword = newPass;

                if (ModelState.IsValid)
                {
                    vsc.Entry(manager).State = EntityState.Modified;
                    vsc.SaveChanges();
                }

                FormsAuthentication.SignOut();
                Response.Cookies.Clear();
                return RedirectToAction("", "Admin");
            }
        }
    }
}
