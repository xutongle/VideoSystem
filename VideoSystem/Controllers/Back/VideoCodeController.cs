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

        public ActionResult Index(int page_id = 1,int codeStatus = -1,string code = null)
        {
            IEnumerable<Code> codeList = null;
           
            //安状态查询
            if (codeStatus != -1)
            {
                codeList = from items in vsc.Codes
                           orderby items.CodeID
                           where items.CodeStatus == codeStatus
                           select items;
            }
            //查找邀请码
            else if (code != null)
            {
                codeList = from items in vsc.Codes
                           where items.CodeValue == code
                           select items;
            }
            //查询全部
            else
            {
                codeList = from items in vsc.Codes
                           orderby items.CodeID
                           select items;
            }
            
            ip.GetCurrentPageData(codeList, page_id);
            return View(ip);
        }

        public ActionResult CreateCodePage() {
            Video[] videoArray = vsc.Videos.ToArray();

            return View(videoArray);
        }

        [HttpPost]
        public ActionResult CreateCode(int videoID, int codeCounts)
        {
            Video video = vsc.Videos.Find(videoID);
            List<string> codeList = icreateCode.createCode(codeCounts, video);

            foreach(string code in codeList)
            {
                Code c = new Code() { CodeStatus = 0, CodeValue = code, Video = video, VideoID = video.VideoID,UserID = -1};
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

            TempData["info"] = "生成成功";
            return RedirectToAction("CreateCodePage", "VideoCode");
        }

        // GET: /VideoCode/ExportExcel

        public ActionResult ExportExcel(int num = 0, int videoID = -1, int[] videoArray = null)
        {
            Code[] codeArray = null;
            string fileName = null;

            //导出单个视频的邀请码
            if (videoID != -1)
            {
                if (num == 0)
                {
                    codeArray = vsc.Codes.Where(c => c.VideoID == videoID && c.CodeStatus == 0).ToArray();
                }
                else {
                    int count = (from item in vsc.Codes
                                 where item.VideoID == videoID && item.CodeStatus == 0
                                 select item).Count();
                    //请求的数量大于已有的数量
                    if (num > count)
                    {
                        TempData["info"] = "邀请码数量不足";
                        return RedirectToAction("", "VideoManager");
                    }

                    codeArray = (from item in vsc.Codes
                                 where item.VideoID == videoID && item.CodeStatus == 0
                                 orderby item.CodeID descending
                                 select item).Take(num);

                }
                fileName = codeArray[0].Video.VideoName;
            }
            //导出多个视频的邀请码
            else if (videoArray != null)
            {
                
            }
            //导出全部视频的邀请码
            else {
                codeArray = (vsc.Codes).ToArray();
            }
            byte[] bytes = ie.WriteExcel(codeArray);

            

            return File(bytes, "application/vnd.ms-excel", fileName+".xls");
        }
    }
}
