using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using VideoSystem.Concrete;
using VideoSystem.Filters;
using VideoSystem.Models;

namespace VideoSystem.Controllers.Back
{
    [CustAuthorize("admin")]
    public class UploadController : Controller
    {
        private VideoSystemContext vsc = new VideoSystemContext();
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
        public ActionResult UploadVideo(int chunk = -1, int chunks = -1)
        {
            string saveUrl = Server.MapPath("/") + "UploadFiles/Videos/";

            //ajax请求，判断文件是否上传，文件断点续传
            if (Request.IsAjaxRequest())
            {
                //请求类型
                string type = Request.Form["type"];
                //上传整个文件之前，检查文件是否上传过，
                if (type == "init")
                {
                    //获取文件校验码
                    string guid = Request.Form["guid"];
                    //从数据库中查找文件的md5值
                    if (Directory.Exists(saveUrl + guid))   //文件夹存在或文件已上传上传
                    {
                        return Content("{['complete':'true']}");
                    }
                    //文件未上传，创建临时保存分块的文件夹
                    Directory.CreateDirectory(saveUrl + guid);
                    return Content("{['complete':'false']}");
                }
                //上传每个分块之前
                if (type == "block")
                {
                    //当前分块的大小
                    int currentBlockSize = Convert.ToInt32(Request.Form["currentBlockSize"]);

                    //判断当前分块是否已上传完成或者是否已上传
                    string[] tempDirectory = Directory.GetDirectories(saveUrl);
                    bool is_exists = System.IO.File.Exists(tempDirectory[0] + chunk);
                    FileInfo fi = new FileInfo(tempDirectory[0] + chunk);
                    if (is_exists && fi.Length == currentBlockSize)
                    {
                        return Content("{['is_exists':'true']}");
                    }
                    //如果分块上传了部分，则删除分块并重新上传
                    if (is_exists && fi.Length < currentBlockSize)
                    {
                        System.IO.File.Delete(tempDirectory[0] + chunk);
                    }
                    return Content("{['is_exists':'false']}");
                }
                //整个文件上传完成后，合并所有分块
                if (type == "merge")
                {
                    string[] tempDirectory = Directory.GetDirectories(saveUrl);
                    string[] blockFileName = Directory.GetFiles(tempDirectory[0]+"/");
                    string videoLocal = uf.CombineFile(blockFileName, saveUrl);

                    return Content("{['videoLocal':" + videoLocal + "]}");
                }
            }

            HttpPostedFileBase file = null;
            try { 
                file = Request.Files[0];
            }
            catch(ArgumentOutOfRangeException e){
                
            }
            
            UploadInfo info = null;

            //is chunked
            if (chunk != -1 && chunks != -1)
            {
                info = uf.UploadVideo(file, saveUrl, chunk, chunks);
            }
            else 
            {
                info = uf.UploadVideo(file, saveUrl, 0, 0);
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string jsonStr = serializer.Serialize(info);

            return Content(jsonStr);
        }
    }
}
