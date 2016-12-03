using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoSystem.Abstract;
using VideoSystem.Filters;
using VideoSystem.Models;

namespace VideoSystem.Controllers.Back
{
    [CustAuthorize("admin")]
    public class UserManagerController : Controller
    {
        private VideoSystemContext vsc = new VideoSystemContext();
        private IPaging ip;

        public UserManagerController(IPaging ip)
        {
            this.ip = ip;
        }

        //
        // GET: /UserManager/

        public ActionResult Index(int page_id = 1)
        {
            IEnumerable<User> userList = from items in vsc.Users
                                         orderby items.UserID
                                         select items;
            ip.GetCurrentPageData(userList, page_id);
            Manager manager = (Manager)Session["Manager"];
            ViewBag.account = manager.ManagerAccount;
            return View(ip);
        }

        //用户反馈列表
        public ActionResult UserSuggestPage(int page_id = 1) 
        {
            IEnumerable<Suggest> suggestList = from items in vsc.Suggests
                                               orderby items.SuggestID
                                               select items;
            Manager manager = (Manager)Session["Manager"];
            ViewBag.account = manager.ManagerAccount;
            ip.GetCurrentPageData(suggestList, page_id);
            return View(ip);
        }
    }
}
