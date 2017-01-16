using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoSystem.Abstract;
using VideoSystem.Filters;
using VideoSystem.Models;

namespace VideoSystem.Controllers.Back
{
    [CustAuthorize("admin")]
    public class VideoManagerController : Controller
    {
        private VideoSystemContext vsc = new VideoSystemContext();
        private IPaging ip;

        public VideoManagerController(IPaging ip)
        {
            this.ip = ip;
        }

        //
        // GET: /VideoManager/

        public ActionResult Index(int page_id = 1)
        {
            IEnumerable<Video> videoList = from items in vsc.Videos
                                           orderby items.UploadTime descending
                                          select items;

            TempData["videoCount"] = (from items in vsc.Videos
                                      select items).Count();

            ip.GetCurrentPageData(videoList, page_id);
            Manager manager = (Manager)Session["Manager"];
            ViewBag.searchAction = "/VideoManager/Index/Page";
            ViewBag.account = manager.ManagerAccount;
            return View(ip);
        }

        //跳转上传视频页面
        public ActionResult UploadPage() {
            Manager manager = (Manager)Session["Manager"];
            ViewBag.account = manager.ManagerAccount;
            return View();
        }

        //上传接收来自客户端的视频信息
        [AllowAnonymous]
        public ActionResult UploadVideo(string videoInfo, string token)
        {
            string[] info = videoInfo.Split('_');
            string video_id = info[0];
            string video_uuid = info[1];
            string video_name = info[2];



            return Content(videoInfo + ":" + token);
        }

        //删除视频
        public ActionResult DeleteVideo(int VideoID)
        {
            Video v = vsc.Videos.Find(VideoID);
            if(ModelState.IsValid)
            {
                vsc.Videos.Remove(v);
                vsc.SaveChanges();

                //删除视频文件和视频首图
                string imgUrl = Server.MapPath(v.VideoImageLocal);
                string videoUrl = Server.MapPath(v.VideoLocal);

                FileInfo img = new FileInfo(imgUrl);
                FileInfo video = new FileInfo(videoUrl);

                img.Delete();
                video.Delete();

                return Content("success");
            }

            return Content("erro");
        }
    }
}
