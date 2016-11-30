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

        public VideoCodeController(IPaging ip, ICreateCode icreateCode)
        {
            this.icreateCode = icreateCode;
            this.ip = ip;
        }

        //
        // GET: /VideoCode/

        public ActionResult Index(int page_id = 1,int videoID = -1,int codeStatus = -1,string code = null)
        {
            IEnumerable<Code> codeList = null;
            //按视频查询
            //if(videoID != -1)
            //{
            //    codeList = from items in vsc.Codes
            //               orderby items.CodeID
            //               where items.VideoID == videoID
            //               select items;
            //}
            //安状态查询
            if (codeStatus != -1)
            {
                codeList = from items in vsc.Codes
                           orderby items.CodeID
                           where items.CodeStatus == codeStatus
                           select items;
            }
            //查找邀请码
            if (code != null)
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
    }
}
