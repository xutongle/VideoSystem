using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using VideoSystem.Concrete;
using VideoSystem.Filters;

namespace VideoSystem.Controllers.Back
{
    [CustAuthorize("admin")]
    public class UploadController : Controller
    {
        private UploadFile uf;


        public UploadController(UploadFile uf)
        {
            this.uf = uf;
        }

        //
        // GET: /Upload/

        public ActionResult Index()
        {
            
            return Content("ok");
        }

        //上传图片
        [HttpPost]
        public ContentResult UploadImage()
        {
            HttpPostedFileBase file = Request.Files[0];
            string saveUrl = Server.MapPath("/") + "UploadFiles/VideoImages/";

            UploadInfo info = uf.UploadImage(file, saveUrl);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string jsonStr = serializer.Serialize(info);


            return Content(jsonStr);
        }


        //上传视频
        [HttpPost]
        public ContentResult UploadVideo(int chunk, int chunks, string guid)
        {
            HttpPostedFileBase file = Request.Files[0];
            string saveUrl = Server.MapPath("/") + "UploadFiles/Videos/";
            UploadInfo info = null;

            //is chunked
            if (Request.Form.AllKeys.Any(m => m == "chunk"))
            {
                info = uf.UploadVideo(file, saveUrl, chunk, chunks, guid);
            }
            else 
            {
                info = uf.UploadVideo(file, saveUrl, 0, 0, null);
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string jsonStr = serializer.Serialize(info);

            return Content(jsonStr);
        }
    }
}
