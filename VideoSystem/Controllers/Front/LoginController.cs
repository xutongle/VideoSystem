using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VideoSystem.Abstract;
using VideoSystem.Filters;
using VideoSystem.Models;

namespace VideoSystem.Controllers.Front
{
    public class LoginController : Controller
    {
        private VideoSystemContext vsc = new VideoSystemContext();
        private IEncryption ie;
        public LoginController(IEncryption ie)
        {
            this.ie = ie;
        }
        //
        // GET: /Login/
        //用户登录页面
        public ActionResult Index()
        {
            //自动登陆

            //if (Request.Cookies.Count > 0)
            //{
            //    string userCookie = Request.Cookies["userCookie"].Value;
            //    Session["role"] = "user";
            //    Session["userCookie"] = userCookie;

            //    string cookie1 = userCookie.Split('-')[0];
            //    string cookie2 = userCookie.Split('-')[1];
            //    User[] user1 = vsc.Users.Where(u => u.UserAccount == cookie2 && u.UserPassword == cookie1).ToArray();
            //    Session["User"] = user1[0];
            //    return RedirectToAction("","Home");
            //}

            return View();
        }

        //用户注销账户
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            //设置cookie过期
            Response.Cookies["userCookie"].Value = "null";
            Response.Cookies["userCookie"].Expires = DateTime.Now.AddDays(-1);

            return RedirectToAction("", "");
        }

        //获取用户浏览器指纹
        public ActionResult GetUserBrowser(string UserBrowser = null)
        {
            if (Session["UserBrowser"] == null)
            {
                Session["UserBrowser"] = UserBrowser;
            }
            return Content("ok");
        }

        //第三方登陆
        public ActionResult ThirdLogin(string code = null)
        {
            string appid = "158562cdb943a4";
            string token = "b2d81a423fe83105d97023303de835fb";
            if (code == null)
            {
                return Content("获取信息失败");
            }

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("appid", appid);
            dic.Add("token", token);
            dic.Add("code", code);
            dic.Add("type", "get_user_info");
            string url = "http://open.51094.com/user/auth.html";
            //获取响应
            string response = SendPost(url, dic);

            JObject jo = (JObject)JsonConvert.DeserializeObject(response);
            string uniq = jo["uniq"].ToString();
            string from = jo["from"].ToString();

            User[] user = vsc.Users.Where(u => u.From == from && u.Uniq == uniq).ToArray();
            string UserBrowser;

            if (Session["UserBrowser"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            UserBrowser = Session["UserBrowser"].ToString();
            //首次登陆，保存账号
            if (user.Length <= 0)
            {
                string md5_uniq = ie.MyMD5(uniq);
                User u = new User();
                u.Uniq = uniq;
                u.From = from;
                u.UserAccount = md5_uniq + from;
                u.UserPassword = md5_uniq;
                u.UserPhone = "null";
                u.UserEmail = "null";
                u.UserBrowser1 = UserBrowser;
                u.UserBrowser2 = "no";
                u.UserBrowser3 = "no";

                if (ModelState.IsValid)
                {
                    vsc.Users.Add(u);
                    vsc.SaveChanges();
                }

                string userCookie = u.UserPassword + "-" + u.UserAccount;
                Session["userCookie"] = userCookie;
                Session["User"] = u;
                Session["role"] = "user";
                Response.Cookies["userCookie"].Value = userCookie;
                Response.Cookies["userCookie"].Expires = DateTime.MaxValue;

                return RedirectToAction("Index", "Home");
            }
            else
            {
                string[] userBrowserArray = { user[0].UserBrowser1, user[0].UserBrowser2, user[0].UserBrowser3 };
                //用户浏览器不是已绑定的
                if (!userBrowserArray.Contains(UserBrowser))
                {
                    //用户绑定浏览器个数已满
                    if (!userBrowserArray.Contains("no"))
                    {
                        return Content("您无权在此电脑登陆");
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

                        string userCookie = user[0].UserPassword + "-" + user[0].UserAccount;
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
                    string userCookie = user[0].UserPassword + "-" + user[0].UserAccount;
                    Session["userCookie"] = userCookie;
                    Session["User"] = user[0];
                    Session["role"] = "user";
                    Response.Cookies["userCookie"].Value = userCookie;
                    Response.Cookies["userCookie"].Expires = DateTime.MaxValue;

                    return RedirectToAction("Index", "Home");
                }
            }
        }

        //发送post请求
        public string SendPost(string url, Dictionary<string, string> dic)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url + "?");
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            #region 添加Post 参数
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容  
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        //账号登陆
        [HttpPost]
        public ActionResult Main(string UserBrowser, string account, string password)
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
                        return Content("nolimit");
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

                        return Content("success");
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

                    return Content("success");
                }
            }
            else
            {
                return Content("error");
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
                //return RedirectToAction("", "Login");
                return Content("success");
            }
            else
            {
                //return RedirectToAction("RegistPage", "Login");
                return Content("failure");
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
