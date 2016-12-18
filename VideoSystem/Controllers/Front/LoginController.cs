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

        //第三方登陆
        public ActionResult ThirdLogin(string code)
        {
            string appid = "158562cdb943a4";
            string token = "b2d81a423fe83105d97023303de835fb";
            if(code == null)
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



            return RedirectToAction("Index", "Home");
        }

        //发送post请求
        public string SendPost(string url, Dictionary<string, string> dic)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url+"?");
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
        public ActionResult Main(string UserBrowser,string account = null, string password = null)
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
