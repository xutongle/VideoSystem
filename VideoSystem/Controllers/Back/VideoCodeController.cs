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

        //生成邀请码
        [HttpPost]
        public ActionResult CreateCode(int codeCounts,int videoID)
        {
            if (videoID != -1)
            {
                Video video = vsc.Videos.Find(videoID);
                List<string> codeList = icreateCode.createCode(codeCounts, video);

                foreach (string code in codeList)
                {
                    Code c = new Code() { CodeStatus = 0, CodeValue = code, VideoID = video.VideoID, UserID = -1 };
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
                return Content("生成成功");
            }
            return Content("生成失败");
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
                    return Content("erro");
                }
                return Content("success");
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
                    int count = (from item in vsc.Codes
                                 where item.CodeStatus == 0
                                 select item).Count();
                    //没有可用的邀请码
                    if (count<=0)
                    {
                        return RedirectToAction("", "VideoManager");
                    }

                    codeArray = (from item in vsc.Codes
                                 orderby item.CodeID descending
                                 where item.CodeStatus == 0
                                 select item).ToArray();
                    fileName = "全部视频";
                }

                //修改邀请码状态
                foreach (Code c in codeArray)
                {
                    c.CodeStatus = 1;
                    if (ModelState.IsValid)
                    {
                        vsc.Entry(c).State = EntityState.Modified;
                        vsc.SaveChanges();
                    }
                }

                //修改视频邀请码数量
                Video[] videoArray = vsc.Videos.ToArray();
                foreach (Video v in videoArray)
                {
                    int codesNotUsed = (from item in vsc.Codes
                                        where item.VideoID == v.VideoID && item.CodeStatus == 1
                                        select item).Count();
                    int codesUsed = v.CodeCounts - codesNotUsed;
                    v.CodeNotUsed = codesNotUsed;
                    v.CodeUsed = codesUsed;

                    if (ModelState.IsValid)
                    {
                        vsc.Entry(v).State = EntityState.Modified;
                        vsc.SaveChanges();
                    }
                }

                byte[] bytes = ie.WriteExcel(codeArray);
                return File(bytes, "application/vnd.ms-excel", fileName + ".xls");
            }
        }
    }
}
