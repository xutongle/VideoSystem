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

        //上传视频
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

                //合并完成后删除分块文件
                string saveUrl = Server.MapPath("/") + "UploadFiles/Videos/";
                string[] tempDirectory = Directory.GetDirectories(saveUrl);
                string[] blockFileName = Directory.GetFiles(tempDirectory[0] + "/");
                foreach (string s in blockFileName)
                {
                    FileInfo blockFileInfo = new FileInfo(s);
                    blockFileInfo.Delete();
                }
                Directory.Delete(tempDirectory[0]);

                return RedirectToAction("UploadPage", "VideoManager");
            }
            else {
                TempData["info"] = "请选择视频首图和要上传的视频文件";
                return RedirectToAction("UploadPage", "VideoManager");
            }
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
