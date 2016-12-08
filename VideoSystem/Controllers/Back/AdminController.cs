﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VideoSystem.Abstract;
using VideoSystem.Filters;
using VideoSystem.Models;

namespace VideoSystem.Controllers.Back
{
    public class AdminController : Controller
    {
        private VideoSystemContext vsc = new VideoSystemContext();
        private IEncryption ie;

        public AdminController(IEncryption ie)
        {
            this.ie = ie;
        }
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        [CustAuthorize("admin")]
        public ActionResult BackMain()
        {
            Manager manager = (Manager)Session["Manager"];
            ViewBag.account = manager.ManagerAccount;
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
                string userCookie = ie.SHA256(password);
                Session["userCookie"] = userCookie;
                Response.Cookies["userCookie"].Value = userCookie;
                Response.Cookies["userCookie"].Expires = DateTime.MaxValue;

                Session["Manager"] = managerArray[0];
                return RedirectToAction("BackMain", "Admin");
            }
        }

        //退出登录
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Response.Cookies.Clear();
            return RedirectToAction("","Admin");
        }

    }
}
