using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hosting;
        public ProductController(ApplicationDbContext db, IWebHostEnvironment hosting)
        {
            _db = db;
            _hosting = hosting;
        }
        //tra ve giao dien quan ly san pham (co phan trang)
        public IActionResult Index(int page = 1)
        {
            var pageSize = 5;
            var currentPage = page;
            var dsSanPham = _db.Products.Include(x => x.Category).ToList();
            //truyen du lieu cho View 
            ViewBag.PageSum = Math.Ceiling((double)dsSanPham.Count / pageSize);
            ViewBag.CurrentPage = currentPage;
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ProductPartial", dsSanPham.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList());
            }
            return View(dsSanPham.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList());
        }
        public IActionResult Delete(int id)
        {
            var sp = _db.Products.Find(id);
            return View(sp);
        }
        public IActionResult DeleteConfirmed(int id)
        {
            var sp = _db.Products.Find(id);
            if (sp.ImageUrl != null)
            {
                var oldFilePath = Path.Combine(_hosting.WebRootPath, sp.ImageUrl);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }
            _db.Products.Remove(sp);
            _db.SaveChanges();
            TempData["success"] = "Delete success";
            return RedirectToAction("Index");
        }
        //Them sp
        public IActionResult Add()
        {
            //truyền danh sách thể loại cho View để sinh ra điều khiển DropDownList
            ViewBag.DSTHELOAI = _db.Categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });
            return View();
        }
        //Xử lý thêm sản phẩm
        [HttpPost]
        public IActionResult Add(Product p, IFormFile ImageUrl)
        {
            if (ImageUrl != null)
            {
                p.ImageUrl = SaveImage(ImageUrl);
            }
            _db.Products.Add(p);
            _db.SaveChanges();
            TempData["success"] = "Product inserted success";
            return RedirectToAction("Index");
        }

        private string SaveImage(IFormFile image)
        {
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var path = Path.Combine(_hosting.WebRootPath, @"images/products");
            var saveFile = Path.Combine(path, filename);
            using (var filestream = new FileStream(saveFile, FileMode.Create))
            {
                image.CopyTo(filestream);
            }
            return @"/images/products/" + filename;
        }
        //sua
        #region Update_Product
        //Hiển thị form cập nhật sản phẩm
        public IActionResult Update(int id)
        {
            var sp = _db.Products.Find(id);
            //truyền danh sách thể loại cho View để sinh ra điều khiển DropDownList
            ViewBag.CategoryList = _db.Categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });
            return View(sp);
        }
        [HttpPost]
        public IActionResult Update(Product product, IFormFile ImageUrl)
        {
            var OldProduct = _db.Products.Find(product.Id);
            if (ImageUrl != null)
            {
                //xử lý upload và lưu ảnh đại diện mới
                product.ImageUrl = SaveImage(ImageUrl);
                //xóa ảnh cũ (nếu có)
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var oldFilePath = Path.Combine(_hosting.WebRootPath, product.ImageUrl);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            }
            else
            {
                product.ImageUrl = OldProduct.ImageUrl;
                // product.ImageUrl=SaveImage(ImageUrl);
            }
            //cập nhật product vào table Product
            OldProduct.Name = product.Name;
            OldProduct.Description = product.Description;
            OldProduct.Price = product.Price;
            OldProduct.CategoryId = product.CategoryId;
            OldProduct.ImageUrl = product.ImageUrl;
            _db.SaveChanges();
            TempData["success"] = "Cập nhật sản phẩm thành công";
            return RedirectToAction("Index");

        }
        #endregion


    }
}
