using System;
using System.Collections.Generic;
using System.Data;
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
                                          orderby items.UploadTime
                                          select items;

            TempData["videoCount"] = (from items in vsc.Videos
                                      select items).Count();

            ip.GetCurrentPageData(videoList, page_id);
            return View(ip);
        }


        public ActionResult UploadPage() {
            return View();
        }

        [HttpPost]
        public ActionResult UploadVideo(Video v) {
            v.UploadTime = DateTime.Now;
            v.CodeCounts = 0;
            v.CodeNotUsed = 0;

            if (ModelState.IsValid)
            {
                vsc.Videos.Add(v);
                vsc.SaveChanges();
                TempData["info"] = "视频添加成功";
                return RedirectToAction("UploadPage", "VideoManager");
            }
            else {
                TempData["info"] = "请选择视频首图和要上传的视频文件";
                return RedirectToAction("UploadPage", "VideoManager");
            }
        }


        //编辑视频
        public ActionResult EditVideo()
        {

            return null;
        }


        //删除视频
        public ActionResult DeleteVideo(int VideoID)
        {
            Video v = vsc.Videos.Find(VideoID);
            if(ModelState.IsValid)
            {
                vsc.Videos.Remove(v);
                vsc.SaveChanges();
                return Content("1");
            }
            return Content("0");
        }
    }
}
