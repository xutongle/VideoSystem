using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoSystem.Abstract;

namespace VideoSystem.Controllers.Back
{
    public class VerifyCodeController : Controller
    {
        private IVerifyCode ivc;

        public VerifyCodeController(IVerifyCode iVerifyCode)
        {
            this.ivc = iVerifyCode;
        }

        //
        // GET: /VerifyCode/
        //生成验证码图片
        public ActionResult Index(string time)
        {
            string code = ivc.CreateValidateCode(5);
            Session["ValidateCode"] = code;
            byte[] bytes = ivc.CreateValidateGraphic(code);
            return File(bytes, "image/jpeg");
        }

        [HttpPost]
        public ActionResult CheckVerifyCode(string verifycode)
        {
            if (Session["ValidateCode"].ToString() != verifycode)
            {
                return Content("验证码不正确，请重新输入!");
            }
            else
            {
                return Content(null);
            }
        }
    }
}
