using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VideoSystem.Abstract;
using VideoSystem.Filters;
using VideoSystem.Models;

namespace VideoSystem.Controllers.Front
{
    public class HomeController : Controller
    {
        private VideoSystemContext vsc = new VideoSystemContext();
        private IEncryption ie;

        public HomeController(IEncryption ie)
        {
            this.ie = ie;
        }

        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        //用户注销账户
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Response.Cookies.Clear();
            return RedirectToAction("", "");
        }

        //跳转用户登录页面
        [CustAuthorize("user")]
        public ActionResult MainPage(string currentVideo = null)
        {
            int UserID = Convert.ToInt32(Request.Cookies["UserID"].Value);
            User user = vsc.Users.Find(UserID);

            Code[] codeArray = (from item in vsc.Codes
                                where item.UserID == user.UserID
                                select item).ToArray();

            TempData["currentVideo"] = currentVideo;
            return View(codeArray);
        }

        //用户登录
        [HttpPost]
        public ActionResult Main(string account, string password)
        {
            password = ie.MyMD5(password);
            User[] user = vsc.Users.Where(u => u.UserAccount == account && u.UserPassword == password).ToArray();

            if (user.Count() > 0)
            {
                FormsAuthentication.SetAuthCookie("User", false);
                Response.Cookies["UserID"].Value = Convert.ToString(user[0].UserID);
                Response.Cookies["UserID"].Expires = DateTime.MaxValue; 

                return RedirectToAction("MainPage", "Home");
            }
            else
            {
                TempData["ErroInfo"] = "账号或密码错误!";
                return RedirectToAction("", "");
            }
        }

        //跳转注册页面
        public ActionResult RegistPage() {
            return View();
        }

       
        //用户注册
        [HttpPost]
        public ActionResult Regist(User user) {

            //用户密码加密存储
            user.UserPassword = ie.MyMD5(user.UserPassword);

            if (ModelState.IsValid)
            {
                vsc.Users.Add(user);
                vsc.SaveChanges();
                return RedirectToAction("", "");
            }
            else
            {
                TempData["ErroInfo"] = "注册失败";
                return RedirectToAction("RegistPage", "Home");
            }
        }

        //判断注册的账号是否存在
        [HttpPost]
        public ContentResult IsRegistAccountExist(string account)
        {
            IEnumerable<User> u = from us in vsc.Users.ToList()
                                  where us.UserAccount == account
                                  select us;
            if (u.Count() == 0)
            {
                return Content("1");
            }
            else
            {
                return Content("2");
            }
        }

        
    }
}
