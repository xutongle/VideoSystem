using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VideoSystem.Abstract;
using VideoSystem.Filters;
using VideoSystem.Models;

namespace VideoSystem.Controllers.Front
{
    [CustAuthorize("user")]
    public class HomeController : Controller
    {
        private VideoSystemContext vsc = new VideoSystemContext();
       
        //
        // GET: /Home/
        //首页
        public ActionResult Index(string currentVideo = null)
        {
            User user = (User)(Session["User"]);

            Code[] codeArray = (from item in vsc.Codes
                                where item.UserID == user.UserID
                                select item).ToArray();

            TempData["currentVideo"] = currentVideo;
            return View(codeArray);
        }

        //产品展示
        public ActionResult Product()
        {
            return View();
        }

        //公司简介
        public ActionResult About()
        {
            return View();
        }

        //联系我们
        public ActionResult Contact()
        {
            return View();
        }

        //播放视频页面
        public ActionResult PlayVideo(int videoID)
        {

            return View(videoID);
        }

        //获取视频代码
        public ActionResult GetVideo(int info)
        {
            Video[] currentVideo = vsc.Videos.Where(v => v.VideoID == info).ToArray();
            

            return Content("<div class='row vid_loac' style='margin-top:30px'>"
                           + "<div class='col-sm-12 col-md-8 col-lg-8  col-md-offset-2 ' style='background-color:white;'>"
                                    + "<h2 style='font-family: microsoft yaHei, 微软雅黑, arial; font-size:18px; color:#72ACE3'>" + currentVideo[0].VideoName+ "</h2>"
                                    + "<video id='video' class='video-js vjs-default-skin vjs-big-play-centered' controls preload='auto' width='100%' data-setup='{}'>"
                                    + "<source src='" + currentVideo[0].VideoLocal+ "' type='video/mp4'/>"
                                    + "</video>"
                                    + "</div>"
                                    + "</div>");       
        }

        //用户反馈
        [HttpPost]
        public ContentResult UserSuggest(string suggestText)
        {
            Suggest suggest = new Suggest();
            suggest.SuggestText = suggestText;
            suggest.CreateTime = DateTime.Now;

            User user = (User)(Session["User"]);

            suggest.UserID = user.UserID;
            if (ModelState.IsValid)
            {
                vsc.Suggests.Add(suggest);
                vsc.SaveChanges();
                return Content("反馈成功");
            }

            return Content("反馈失败");
        }

        //获取视频
        [HttpPost]
        public ActionResult GetVideo(string videoCode)
        {
            Code[] isCodeExist = vsc.Codes.Where(code => code.CodeValue == videoCode && code.CodeStatus == 1 && code.UserID == -1).ToArray();
            //邀请码无效
            if (isCodeExist.Length <= 0)
            {
                TempData["info"] = "验证码无效";
                return RedirectToAction("Index", "Home");
            }
            //邀请码存在,但是用户已经有此邀请码对应的视频
            Code c = isCodeExist[0];
            User user = (User)(Session["User"]);
            Code[] codeArray = vsc.Codes.Where(code => code.UserID == user.UserID).ToArray();

            int isExist = codeArray.Where(code2 => code2.VideoID == c.VideoID).ToArray().Length;
            if (isExist != 0)
            {
                c.CodeStatus = 3;
            }
            else
            {
                c.UserID = user.UserID;
                c.CodeStatus = 2;
            }

            if (ModelState.IsValid)
            {
                vsc.Entry(c).State = EntityState.Modified;
                vsc.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }
        
        //修改密码
        [HttpPost]
        public ActionResult ModifyPass(string oldPass, string newPass)
        {
            User user = (User)(Session["User"]);

            if (oldPass != user.UserPassword)
            {
                return Content("erro");
            }

            user.UserPassword = newPass;

            if(ModelState.IsValid)
            {
                vsc.Entry(user).State = EntityState.Modified;
                vsc.SaveChanges();
            }

            FormsAuthentication.SignOut();
            Session.Clear();
            Response.Cookies.Clear();
            return Content("success");
        }
        
    }
}
