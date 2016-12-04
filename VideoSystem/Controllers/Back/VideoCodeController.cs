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
    public class VideoCodeController : Controller
    {
        private VideoSystemContext vsc = new VideoSystemContext();
        private IPaging ip;
        private ICreateCode icreateCode;
        private IExportExcel ie;

        public VideoCodeController(IPaging ip, ICreateCode icreateCode,IExportExcel ie)
        {
            this.icreateCode = icreateCode;
            this.ip = ip;
            this.ie = ie;
        }

        //
        // GET: /VideoCode/

        public ActionResult Index(int page_id = 1)
        {
            IEnumerable<Code> codeList = null;
            
            codeList = from items in vsc.Codes
                       orderby items.CodeID
                       select items;
            
            TempData["codeCount"] = (from items in vsc.Codes
                                     select items).Count();

            TempData["codeCountNotExport"] = (from items in vsc.Codes
                                     where items.CodeStatus == 0
                                     select items).Count();

            TempData["codeCountUsed"] = (from items in vsc.Codes
                                         where items.CodeStatus == 2
                                        select items).Count();

            ip.GetCurrentPageData(codeList, page_id);

            Manager manager = (Manager)Session["Manager"];
            ViewBag.searchAction = "/VideoCode/Index/Page";
            ViewBag.account = manager.ManagerAccount;
            return View(ip);
        }

        public ActionResult CreateCodePage() {
            Video[] videoArray = vsc.Videos.ToArray();

            Manager manager = (Manager)Session["Manager"];
            ViewBag.account = manager.ManagerAccount;
            return View(videoArray);
        }

        [HttpPost]
        public ActionResult CreateCode(int codeCounts,int videoID)
        {
            if (videoID != -1)
            {
                Video video = vsc.Videos.Find(videoID);
                List<string> codeList = icreateCode.createCode(codeCounts, video);

                foreach (string code in codeList)
                {
                    Code c = new Code() { CodeStatus = 0, CodeValue = code, Video = video, VideoID = video.VideoID, UserID = -1 };
                    if (ModelState.IsValid)
                    {
                        vsc.Codes.Add(c);
                        vsc.SaveChanges();
                    }

                }

                video.CodeNotUsed += codeCounts;
                if (ModelState.IsValid)
                {
                    vsc.Entry(video).State = EntityState.Modified;
                    vsc.SaveChanges();
                }
                return Content("1");
            }
            

            return Content("0");
        }

        // GET: /VideoCode/ExportExcel
        //导出邀请码
        public ActionResult ExportExcel(int num = 0, int videoID = -1)
        {
            Code[] codeArray = null;
            string fileName = null;


            if (Request.IsAjaxRequest())
            {
                int count = (from item in vsc.Codes
                             where item.VideoID == videoID && item.CodeStatus == 0
                             select item).Count();
                //请求的数量大于已有的数量
                if (num > count)
                {
                    return Content("1");
                }
                return Content("0");
            }
            else {
                //导出单个视频的邀请码
                if (videoID != -1)
                {
                    if (num == 0)
                    {
                        codeArray = vsc.Codes.Where(c => c.VideoID == videoID && c.CodeStatus == 0).ToArray();
                    }
                    else
                    {
                        int count = (from item in vsc.Codes
                                     where item.VideoID == videoID && item.CodeStatus == 0
                                     select item).Count();
                        //请求的数量大于已有的数量
                        if (num > count)
                        {
                            return RedirectToAction("", "VideoManager");
                        }

                        codeArray = (from item in vsc.Codes
                                     where item.VideoID == videoID && item.CodeStatus == 0
                                     orderby item.CodeID descending
                                     select item).Take(num).ToArray();

                    }
                    fileName = codeArray[0].Video.VideoName;
                }
                //导出全部视频的邀请码
                else
                {
                    codeArray = (from item in vsc.Codes
                                 orderby item.CodeID descending
                                 select item).ToArray();
                    fileName = "全部视频";
                }


                foreach (Code c in codeArray)
                {
                    c.CodeStatus = 1;
                    if (ModelState.IsValid)
                    {
                        vsc.Entry(c).State = EntityState.Modified;
                        vsc.SaveChanges();
                    }
                }

                byte[] bytes = ie.WriteExcel(codeArray);
                return File(bytes, "application/vnd.ms-excel", fileName + ".xls");
            }
        }
    }
}
