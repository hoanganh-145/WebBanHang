using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Models;
using WebBanHang.Areas.Customer.Models;
namespace WebBanHang.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(int catid = 1)
        {
            var dsLoai = _db.Categories.OrderBy(x => x.DisplayOrder)
                .Select (c=> new CategoryViewModel { Id = c.Id, Name = c.Name,
                                                    ToTalProduct = _db.Products.Where(p => p.CategoryId==c.Id).Count()})
                .ToList();

            var dsSanPham = _db.Products.Where(p => p.CategoryId == catid).ToList();

            var categoryName = _db.Categories.Find(catid).Name;

            ViewBag.DSLOAI = dsLoai;
            ViewBag.CATEGORY_NAME = categoryName;
            return View(dsSanPham);
        }
        //Tra ve view khong can layout
        public IActionResult LoadPartial (int catid = 1)
        {
         
            var dsSanPham = _db.Products.Where(p => p.CategoryId == catid).ToList();

            var categoryName = _db.Categories.Find(catid).Name;

            ViewBag.CATEGORY_NAME = categoryName;
            return PartialView("_ProductPartial",dsSanPham);
        }
    }
}
