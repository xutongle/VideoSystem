using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VideoSystem.Concrete;

namespace VideoSystem.Abstract
{
    public interface IUploadFile
    {
        UploadInfo UploadImage(HttpPostedFileBase file, string saveLocal);
        UploadInfo UploadVideo(HttpPostedFileBase file, string saveLocal, int chunk, int chunks);
    }
}
