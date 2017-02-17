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
using VideoSystem.Models.LCUtils;

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

        //上传视频首图
        public ActionResult UploadImg(int videoID,string VideoImageLocal)
        {
            Video v = vsc.Videos.Find(videoID);
            v.VideoImageLocal = VideoImageLocal;

            if (ModelState.IsValid)
            {
                vsc.Entry(v).State = EntityState.Modified;
                vsc.SaveChanges();
            }

            return RedirectToAction("Index", "VideoManager");
        }

        //跳转上传视频页面
        public ActionResult UploadPage(int VideoID) {
            Manager manager = (Manager)Session["Manager"];
            ViewBag.account = manager.ManagerAccount;

            return View(vsc.Videos.Find(VideoID));
        }

        //上传接收来自客户端的视频信息
        [AllowAnonymous]
        public ActionResult UploadVideo(string videoInfo, string token)
        {
            if (token != null & token == "123456789")
            {
                string[] info = videoInfo.Split('_');
                string video_id = info[0];
                string video_uuid = info[1];
                string video_name = info[2];

                Video v = new Video();
                v.CodeCounts = 0;
                v.CodeNotUsed = 0;
                v.CodeUsed = 0;
                v.ls_video_id = int.Parse(video_id);
                v.ls_video_uuid = video_uuid;
                v.VideoName = video_name;
                v.VideoImageLocal = "null";
                v.UploadTime = DateTime.Now;

                if (ModelState.IsValid)
                {
                    vsc.Videos.Add(v);
                    vsc.SaveChanges();
                    return Content("ok");
                }
            }

            return Content("error");
        }

        //删除视频
        public ActionResult DeleteVideo(int VideoID)
        {
            Video v = vsc.Videos.Find(VideoID);
            if (ModelState.IsValid)
            {
                vsc.Videos.Remove(v);
                vsc.SaveChanges();

                //删除视频文件和视频首图
                if (v.VideoImageLocal != "null")
                {
                    string imgUrl = Server.MapPath(v.VideoImageLocal);
                    FileInfo img = new FileInfo(imgUrl);
                    img.Delete();
                }

                LCUtils lc = new LCUtils();
                jsonout result = lc.deleteVideo(v.ls_video_id);

                if (result.code == "0")
                {
                    return Content("success");
                }
                else {
                    return Content("erro");
                }
            }

            return Content("erro");
        }

        public ActionResult getCode(int VideoID = -1, int page_id = 1)
        {
            if (VideoID != -1)
            {
                Session["VideoID"] = VideoID;
            }
            else {
                VideoID = (int)Session["VideoID"];
            }

            IEnumerable<Code> codeList = null;

            codeList = from items in vsc.Codes
                       where items.VideoID == VideoID
                       orderby items.CodeID
                       select items;

            ip.GetCurrentPageData(codeList, page_id);

            TempData["codeCount"] = (from items in vsc.Codes
                                     where items.VideoID == VideoID
                                     select items).Count();

            TempData["codeCountNotExport"] = (from items in vsc.Codes
                                              where items.CodeStatus == 0 && items.VideoID == VideoID
                                              select items).Count();

            TempData["codeCountUsed"] = (from items in vsc.Codes
                                         where items.CodeStatus == 2 && items.VideoID == VideoID
                                         select items).Count();

            Manager manager = (Manager)Session["Manager"];
            ViewBag.searchAction = "/VideoManager/getCode/Page";
            ViewBag.account = manager.ManagerAccount;
            return View(ip);
        }
    }
}
