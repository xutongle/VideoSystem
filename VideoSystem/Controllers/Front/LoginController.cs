using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VideoSystem.Filters;
using VideoSystem.Models;

namespace VideoSystem.Controllers.Front
{
    public class LoginController : Controller
    {
        private VideoSystemContext vsc = new VideoSystemContext();

        //
        // GET: /Login/
        //用户登录页面
        public ActionResult Index()
        {
            return View();
        }

        //用户注销账户
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Response.Cookies.Clear();
            return RedirectToAction("", "");
        }

        //登陆
        [HttpPost]
        public ActionResult Main(string account, string password, string UserBrowser)
        {
            User[] user = vsc.Users.Where(u => u.UserAccount == account && u.UserPassword == password).ToArray();
            if (user.Count() > 0)
            {
                string[] userBrowserArray = { user[0].UserBrowser1, user[0].UserBrowser2, user[0].UserBrowser3 };

                //用户浏览器不是已绑定的
                if (!userBrowserArray.Contains(UserBrowser))
                {
                    if (!userBrowserArray.Contains("no"))
                    {
                        TempData["ErroInfo"] = "您无权在当前电脑登录!";
                        return RedirectToAction("", "");
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (userBrowserArray[i] == "no")
                            {
                                userBrowserArray[i] = UserBrowser;
                                break;
                            }
                        }

                        user[0].UserBrowser1 = userBrowserArray[0];
                        user[0].UserBrowser2 = userBrowserArray[1];
                        user[0].UserBrowser3 = userBrowserArray[2];

                        if (ModelState.IsValid)
                        {
                            vsc.Entry(user[0]).State = EntityState.Modified;
                            vsc.SaveChanges();
                        }

                        string userCookie = password + "-" + account;
                        Session["userCookie"] = userCookie;
                        Session["User"] = user[0];
                        Session["role"] = "user";
                        Response.Cookies["userCookie"].Value = userCookie;
                        Response.Cookies["userCookie"].Expires = DateTime.MaxValue;

                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    string userCookie = password + "-" + account;
                    Session["userCookie"] = userCookie;
                    Session["User"] = user[0];
                    Session["role"] = "user";
                    Response.Cookies["userCookie"].Value = userCookie;
                    Response.Cookies["userCookie"].Expires = DateTime.MaxValue;

                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["ErroInfo"] = "账号或密码错误!";
                return RedirectToAction("", "");
            }
        } 

        //跳转注册页面
        public ActionResult RegistPage()
        {
            return View();
        }

        //用户注册
        [HttpPost]
        public ActionResult Regist(User user)
        {

            if (ModelState.IsValid)
            {
                vsc.Users.Add(user);
                vsc.SaveChanges();
                return RedirectToAction("", "");
            }
            else
            {
                TempData["ErroInfo"] = "注册失败";
                return RedirectToAction("RegistPage", "Login");
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
                return Content("success");
            }
            else
            {
                //账号已存在
                return Content("erro");
            }
        }
    }
}
