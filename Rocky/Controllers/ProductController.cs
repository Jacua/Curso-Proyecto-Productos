using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rocky.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        private readonly IWebHostEnvironment _webHostEnviaronment;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;

            _webHostEnviaronment = hostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _db.Product.ToList();

            return View(products);
        }

        //GET UPSERT
        [HttpGet]
         public IActionResult UpSert(int? id)
        {
            ProductCategoryViewModel productCategory = new ProductCategoryViewModel()
            {
                Product = new Product(),
                categorySelectList = _db.Category.Select(x => new SelectListItem {

                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            if (id > 0 && id != null)
            {
                //Update Acction

                productCategory.Product = _db.Product.FirstOrDefault(x => x.Id == id);



                return View(productCategory);
                
            }

            return View(productCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpSert(ProductCategoryViewModel productCategory)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnviaronment.WebRootPath;

                if (productCategory.Product.Id == 0)
                {
                    //Create new Product

                    string upload = webRootPath+WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload,fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);

                    }

                    productCategory.Product.Image = fileName + extension;

                    _db.Product.Add(productCategory.Product);
                }
                else
                {
                    //update product
                }
            }

            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
