using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoSystem.Abstract;
using VideoSystem.Models;

namespace VideoSystem.Controllers.Back
{
    public class ProductController : Controller
    {
        private VideoSystemContext vsc = new VideoSystemContext();
        private IPaging ip;
        public ProductController(IPaging ip)
        {
            this.ip = ip;
        }

        //
        // GET: /Product/
        //产品列表页面
        public ActionResult Index(int page_id = 1)
        {
            TempData["productCount"] = (from items in vsc.Products
                                      select items).Count();

            IEnumerable<Product> productList = from item in vsc.Products
                                               orderby item.ProductID descending
                                               select item;

            ip.GetCurrentPageData(productList, page_id);
            Manager manager = (Manager)Session["Manager"];
            ViewBag.searchAction = "/Product/Index/Page";
            ViewBag.account = manager.ManagerAccount;
            return View(ip);
        }

        //添加产品页面
        public ActionResult AddProductPage()
        {
            Manager manager = (Manager)Session["Manager"];
            ViewBag.account = manager.ManagerAccount;
            return View();
        }

        //添加产品
        [HttpPost]
        public ActionResult AddProduct(Product p)
        {
            if(ModelState.IsValid)
            {
                vsc.Products.Add(p);
                vsc.SaveChanges();
                TempData["info"] = "添加成功";
                return RedirectToAction("AddProductPage", "Product");
            }
            TempData["info"] = "添加失败";
            return RedirectToAction("AddProductPage", "Product");
        }

        //返回编辑产品页面
        public ActionResult EditProductPage(int productID)
        {
            Product p = vsc.Products.Find(productID);
            return View(p);
        }

        //编辑产品
        [HttpPost]
        public ActionResult EditProduct(Product p)
        {
            Product oldProduct = vsc.Products.Find(p.ProductID);

            if (oldProduct.ProductImg != p.ProductImg)
            {
                 //删除原来的图片
                string imgUrl = Server.MapPath(oldProduct.ProductImg);
                FileInfo img = new FileInfo(imgUrl);
                img.Delete();
            }

            oldProduct.ProductName = p.ProductName;
            oldProduct.ProductPrice = p.ProductPrice;
            oldProduct.ProductUrl = p.ProductUrl;
            oldProduct.ProductImg = p.ProductImg;

            if (ModelState.IsValid)
            {
                vsc.Entry(oldProduct).State = EntityState.Modified;
                vsc.SaveChanges();
                return Content("编辑成功");
            }
            return Content("编辑失败");
        }

        //删除产品
        public ActionResult DeleteProduct(int productID)
        {
            Product p = vsc.Products.Find(productID);
            if (ModelState.IsValid)
            {
                vsc.Products.Remove(p);
                vsc.SaveChanges();

                //删除视频对应的图片
                string imgUrl = Server.MapPath(p.ProductImg);
                FileInfo img = new FileInfo(imgUrl);
                img.Delete();
                return Content("scuuecc");
            }
            return Content("failure");
        }
    }
}
