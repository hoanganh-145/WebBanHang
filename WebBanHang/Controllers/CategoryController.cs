using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Models;
namespace WebBanHang.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;       
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;        
        }
        //action Index() : tra ve giao dien quan ly the loai
        public IActionResult Index()
        {
            var listCategory = _db.Categories.ToList();
            return View(listCategory);
        }
        public IActionResult Delete(int id)
        {
            var category = _db.Categories.Where(x => x.Id == id).FirstOrDefault();
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost, ActionName("A")]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = _db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            if(_db.Products.Where(x=>x.CategoryId==id).ToList().Count>0) //kiem tra co ton tai san pham nao theo the loai can xoa
            {
                TempData["error"] = "The loai can xoa da co san pham. Khong the xoa";
                return RedirectToAction("Index");
            }    
            _db.Categories.Remove(category);
            _db.SaveChanges();
            TempData["success"] = "Category deleted success";
            return RedirectToAction("Index");
        }
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Add(category);
                _db.SaveChanges();
                TempData["success"] = "Category inserted success";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Update(int id)
        {
            var category = _db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Update(Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Update(category);
                _db.SaveChanges();
                TempData["success"] = "Category updated success";
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
