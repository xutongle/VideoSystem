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
            ViewBag.searchAction = "/UserManager/Index/Page";
            ViewBag.account = manager.ManagerAccount;
            return View(ip);
        }

        //用户反馈列表
        public ActionResult UserSuggestPage(int page_id = 1) 
        {
            IEnumerable<Suggest> suggestList = from items in vsc.Suggests
                                               orderby items.CreateTime descending
                                               select items;
            Manager manager = (Manager)Session["Manager"];
            ViewBag.account = manager.ManagerAccount;
            ViewBag.searchAction = "/UserSuggestPage/Index/Page";
            ip.GetCurrentPageData(suggestList, page_id);
            return View(ip);
        }

        //删除用户
        public ActionResult DeleteUser(int userID)
        {
            User user = vsc.Users.Find(userID);
            if(ModelState.IsValid)
            {
                vsc.Users.Remove(user);
                vsc.SaveChanges();
            }
            Code[] codeList = (from item in vsc.Codes
                               where item.UserID == userID
                                select item).ToArray();
            //删除用户对应的邀请码
            if (ModelState.IsValid)
            {
                foreach (Code c in codeList)
                {
                    vsc.Codes.Remove(c);
                }
                vsc.SaveChanges();
            }
            return RedirectToAction("", "UserManager");
        }

        //查看用户视频
        public ActionResult GetUserVideo(int userID)
        {
            Code[] CodeArraay = (from item in vsc.Codes
                                 where item.UserID == userID
                                 select item).ToArray();

            return View(CodeArraay);
        }

        //清除绑定
        public ActionResult ClearBroser(string userID)
        {
            User user = vsc.Users.Find(userID);

            return null;
        }
    }
}
