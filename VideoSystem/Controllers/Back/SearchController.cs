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
    public class SearchController : Controller
    {
        private VideoSystemContext vsc = new VideoSystemContext();
        private IPaging ip;

        public SearchController(IPaging ip)
        {
            this.ip = ip;
        }

        // GET: /Search/

        public ActionResult Index(string model, string searchType, string searchValue, int page_id = 1)
        {
            if (string.IsNullOrEmpty(model) || string.IsNullOrEmpty(searchType) || string.IsNullOrEmpty(searchValue))
            {
                model = (string)Session["search_model"];
                searchType = (string)Session["searchType"];
                searchValue = (string)Session["searchValue"];
            }
            else {
                Session["search_model"] = model;
                Session["searchType"] = searchType;
                Session["searchValue"] = searchValue;
            }

            //查询邀请码
            if (model == "Code")
            {
                IEnumerable<Code> codeList = null;

                //按邀请码查询
                if (searchType == "code")
                {
                    codeList = from items in vsc.Codes
                               where items.CodeValue == searchValue
                               select items;
                }
                //按状态查询
                if (searchType == "status")
                {
                    int statusInt = Convert.ToInt32(searchValue);
                    codeList = from items in vsc.Codes
                               orderby items.CodeID
                               where items.CodeStatus == statusInt
                               select items;
                }
                //按视频查询
                if (searchType == "video")
                {
                    codeList = from items in vsc.Codes
                               orderby items.CodeID
                               where items.Video.VideoName.Contains(searchValue)
                               select items;
                }

                ip.GetCurrentPageData(codeList, page_id);
                ViewBag.searchAction = "/Search/Index/Page";
                return View("/Views/Back/VideoCode/Index.cshtml", ip);
            }
            //查询视频
            if (model == "Video")
            {
                IEnumerable<Video> videoList = null;

                //按编号
                if (searchType == "videoID")
                {
                    int videoID = Convert.ToInt32(searchValue);
                    videoList = from items in vsc.Videos
                                where items.VideoID == videoID
                                select items;
                }
                //按名称
                if (searchType == "videoName")
                {
                    videoList = from items in vsc.Videos
                                orderby items.VideoID
                                where items.VideoName.Contains(searchValue)
                                select items;
                }

                ip.GetCurrentPageData(videoList, page_id);
                ViewBag.searchAction = "/Search/Index/Page";
                return View("/Views/Back/VideoManager/Index.cshtml",ip);
            }
            //查询建议
            if (model == "Suggest")
            {
                IEnumerable<Suggest> suggestList = null;

                //内容
                if (searchType == "suggestValue")
                {
                    suggestList = from item in vsc.Suggests
                                  where item.SuggestText.Contains(searchValue)
                                  select item;
                }

                //账号
                if (searchType == "account")
                {
                    suggestList = from item in vsc.Suggests
                                  where item.User.UserAccount.Contains(searchValue)
                                  select item;
                }

                //邮箱
                if (searchType == "email")
                {
                    suggestList = from item in vsc.Suggests
                                  where item.User.UserEmail.Contains(searchValue)
                                  select item;
                }

                //电话
                if (searchType == "phone")
                {
                    suggestList = from item in vsc.Suggests
                                  where item.User.UserPhone.Contains(searchValue)
                                  select item;
                }

                ip.GetCurrentPageData(suggestList, page_id);
                ViewBag.searchAction = "/Search/Index/Page";
                return View("/Views/Back/UserManager/UserSuggestPage.cshtml",ip);
            }
            //查询用户
            if (model == "User")
            {
                IEnumerable<User> userList = null;

                //账号
                if (searchType == "account")
                {
                    userList = from item in vsc.Users
                               where item.UserAccount.Contains(searchValue)
                               select item;
                }

                //邮箱
                if (searchType == "email")
                {
                    userList = from item in vsc.Users
                               where item.UserEmail.Contains(searchValue)
                               select item;
                }

                //电话
                if (searchType == "phone")
                {
                    userList = from item in vsc.Users
                               where item.UserPhone.Contains(searchValue)
                               select item;
                }

                ip.GetCurrentPageData(userList, page_id);
                ViewBag.searchAction = "/Search/Index/Page";
                return View("/Views/Back/UserManager/Index.cshtml",ip);
            }
            return null;
        }
    }
}
