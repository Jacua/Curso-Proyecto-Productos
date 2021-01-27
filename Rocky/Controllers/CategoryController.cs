using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController( ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> ObjCategory = _db.Category.ToList();
            
            return View(ObjCategory);
        }

        // CREATE GET
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create( Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Category.Add(category);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(category);
        }

        //EDIT GET

        public IActionResult Edit(int? id)
        {
            if (id != 0 || id != null) {

                var objCategory = _db.Category.FirstOrDefault(x => x.Id == id);

                return View(objCategory);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                var objCategory = _db.Category.FirstOrDefault(x => x.Id == category.Id);
                objCategory.Name = category.Name;
                objCategory.DisplayOrder = category.DisplayOrder;

                _db.SaveChanges();

                _db.Dispose();

                return RedirectToAction("Index");
            }

            return View(category);
        }
    }
}
