using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Web;
using System.Web.Mvc;

namespace VideoSystem.Controllers.Back
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult Index()
        {
            ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection queryCollection = query.Get();
            string mac = null;
            foreach (ManagementObject mo in queryCollection)
            {
                if (mo["IPEnabled"].ToString() == "True")
                {
                    mac = mo["MacAddress"].ToString();
                    break;
                }
            }


            return Content(mac);
        }

    }
}
