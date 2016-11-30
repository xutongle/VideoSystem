using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using VideoSystem.Abstract;

namespace VideoSystem.Concrete
{
    public class UploadFile : IUploadFile
    {
        public UploadInfo UploadImage(HttpPostedFileBase file, string saveLocal)
        {
            UploadInfo info = new UploadInfo();

            //文件保存路径
            string saveUrl = saveLocal;
            //文件名称
            string imageName="";
            //文件后缀
            string imageExt = "";
            //文件类型
            string[] fileTypes = {"jpeg","jpg","gif","png","bmp"};
            //文件大小限制5M
            int fileSize = 3*1024*1024;

            //判断文件是否为空
            if (file == null)
            {
                info.statuCode = 400;
                info.info = "文件为空";
                return info;
            }

            //判断文件类型
            imageExt = getFileSuffix(file.FileName);
            if (!fileTypes.Contains(imageExt))
            {
                info.statuCode = 401;
                info.info = "文件格式不支持";
                return info;
            }

            //判断文件大小
            if (file.ContentLength > fileSize)
            {
                info.statuCode = 403;
                info.info = "文件大小超过限制";
                return info;
            }

            //生成文件名称
            imageName = createImageName();
            
            //判断路径是否存在
            if (!Directory.Exists(saveUrl))
            {
                //如果不存在则生成目录
                Directory.CreateDirectory(saveUrl);
            }
            //保存文件
            file.SaveAs(saveUrl + imageName + "." + imageExt);
            info.statuCode = 200;
            info.info = "/UploadFiles/VideoImages/" + imageName + "." + imageExt;
            return info;
        }

        public UploadInfo UploadVideo(HttpPostedFileBase file, string saveLocal,int chunk,int chunks,string guid)
        {
            UploadInfo info = new UploadInfo();
            string videoName = null;
            string fileSuffix = getFileSuffix(file.FileName);

            //文件没有分块，直接保存
            if (chunk == 0 && chunks == 0)
            {
                videoName = createImageName();
                file.SaveAs(saveLocal + videoName + "." + fileSuffix);
            }
            else
            {
                //文件分块了
                FileStream addFile = new FileStream(saveLocal+guid,FileMode.Append,FileAccess.Write);
                BinaryWriter addWriter = new BinaryWriter(addFile);

                Stream stream = file.InputStream;
                BinaryReader tempReader = new BinaryReader(stream);
                addWriter.Write(tempReader.ReadBytes((int)(stream.Length)));

                tempReader.Close();
                stream.Close();
                addWriter.Close();
                addFile.Close();

                tempReader.Dispose();
                stream.Dispose();
                addWriter.Dispose();
                addFile.Dispose();

                if(chunk == (chunks-1))
                {
                    videoName = createImageName();
                    FileInfo fileInfo = new FileInfo(saveLocal+guid);
                    fileInfo.MoveTo(saveLocal + videoName + "." + fileSuffix);
                }
                
            }
            info.statuCode = 200;
            info.info = "/UploadFiles/Videos/" + videoName + "." + fileSuffix;
            return info;
        }

        //获取文件后缀
        private string getFileSuffix(string fileName)
        {
            string suffix = null;
            if (fileName.IndexOf('.')>0)
            {
                string[] fs = fileName.Split('.');
                suffix = fs[fs.Length - 1]; 
            }
            return suffix;
        }

        //生成图片名称
        private string createImageName()
        {
            string imageName = "";
            string timeTicks = DateTime.Now.ToString("yyMMddHHmmssfff");
            Random seekRand = new Random();
            imageName += timeTicks;
            imageName += seekRand.Next(10000, 99999);
            return imageName;
        }
    }

    public class UploadInfo {
        public int statuCode;
        public string info;
    }
}