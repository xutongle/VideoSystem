
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using VideoSystem.Models;

namespace VideoSystem.Filters
{
    public class CustAuthorizeAttribute : AuthorizeAttribute
    {
        private VideoSystemContext vsc = new VideoSystemContext();
        private string[] roles;
  
        public CustAuthorizeAttribute(params String[] role)
        {
              roles = role;
        }

        //用户授权规则
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            HttpRequestBase r = httpContext.Request;
            if (httpContext.Session["role"] == null)
            {
                return false;
            }
            
            try {
                string userCookie = null;
                string adminCookie = null;

                if (r.Cookies["userCookie"] != null)
                {
                    userCookie = r.Cookies["userCookie"].Value;
                }
                if (r.Cookies["adminCookie"] != null)
                {
                    adminCookie = r.Cookies["adminCookie"].Value;
                }

                
                //用户是管理员
                if (roles[0] == "admin")
                {
                    if (adminCookie != null)
                    {
                        string[] adminInfo = adminCookie.Split('-');
                        string adminAccount = adminInfo[1];
                        string adminPass = adminInfo[0];
                        Manager[] m = vsc.Managers.Where(manager => manager.ManagerPassword == adminPass && manager.ManagerAccount == adminAccount).ToArray();
                        if (m.Length <= 0)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                        return false;

                }
                //一般用户
                else
                {
                    if (userCookie != null)
                    {
                        string[] userInfo = userCookie.Split('-');
                        string account = userInfo[1];
                        string password = userInfo[0];
                        User[] u = vsc.Users.Where(user => user.UserPassword == password && user.UserAccount == account).ToArray();
                        if (u.Length <= 0)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                        return false;
                }
            }
            catch(Exception)
            {
                return false;
            }
        }

        //处理未能授权的情况
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (roles[0] == "admin")
            {
                filterContext.Result = new RedirectResult("~/Qhgypacz");
            }
            if (roles[0] == "user")
            {
                filterContext.Result = new RedirectResult("~/Login");
            }
        }
    }
}