
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace VideoSystem.Filters
{
    public class CustAuthorizeAttribute : AuthorizeAttribute
    {
        private string[] roles;
  
        public CustAuthorizeAttribute(params String[] role)
        {
              roles = role;
        }

        //用户授权规则
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            HttpRequestBase r = httpContext.Request;

            string c = r.Cookies["userCookie"].Value;
            if (c != null)
            {
                if (httpContext.Session["userCookie"] != c)
                {
                    return false;
                }
                return true;
            }
            return false;
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