using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoSystem.Filters;
using VideoSystem.Models;

namespace VideoSystem.Controllers.Front
{
    public class UserMainController : Controller
    {
        private VideoSystemContext vsc = new VideoSystemContext();

        // GET: /UserMain/

        public ActionResult Index()
        {
            return View();
        }


        //用户反馈
        [CustAuthorize("user")]
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
        [CustAuthorize("user")]
        [HttpPost]
        public ActionResult GetVideo(string videoCode)
        {
            Code[] isCodeExist = vsc.Codes.Where(code => code.CodeValue == videoCode).ToArray();
            //邀请码无效
            if (isCodeExist.Length <= 0)
            {
                TempData["info"] = "验证码无效";
                return RedirectToAction("MainPage", "Home");
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

            return RedirectToAction("MainPage", "Home");
        }
    }
}
