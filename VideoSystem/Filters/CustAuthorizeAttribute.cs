
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
            try {
                string userCookie = r.Cookies["userCookie"].Value;
                string[] info = userCookie.Split('-');

                if (userCookie != null)
                {
                    string account = info[1];
                    string password = info[0];
                    //用户是管理员
                    if (roles[0] == "admin")
                    {
                        Manager[] m = vsc.Managers.Where(manager => manager.ManagerPassword == password && manager.ManagerAccount == account).ToArray();
                        if (m.Length <= 0)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    //一般用户
                    else
                    {
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
                }
                else {
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
                filterContext.Result = new RedirectResult("~/Admin");
            }
            if (roles[0] == "user")
            {
                filterContext.Result = new RedirectResult("~/Home");
            }
        }
    }
}