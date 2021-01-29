using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
            //Eager Loading
            IEnumerable<Product> products = _db.Product.Include(x=> x.Category).Include(x => x.ApplicationType);



            //Mala Practica, demasiadas llamadas a la base de datos
            //foreach (var obj in products)
            //{
            //    obj.Category = _db.Category.FirstOrDefault(u => u.Id == obj.CategoryId);
            //    obj.ApplicationType = _db.ApplicationType.FirstOrDefault(u => u.Id == obj.ApplicationTypeId);
            //}

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
                }),
                ApplicationTypeSelecList = _db.ApplicationType.Select(x => new SelectListItem {
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

                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);

                    }

                    productCategory.Product.Image = fileName + extension;

                    _db.Product.Add(productCategory.Product);
                }
                else
                {
                    //update product
                    var objProduct = _db.Product.AsNoTracking().FirstOrDefault(x => x.Id == productCategory.Product.Id);

                    // actualizar imagen y borrar antigua

                    if (files.Count() > 0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, objProduct.Image);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productCategory.Product.Image = fileName + extension;

                    }
                    else
                    {
                        productCategory.Product.Image = objProduct.Image;
                    }

                    _db.Product.Update(productCategory.Product);
                }

                _db.SaveChanges();

                return RedirectToAction("Index");

            }

            // if model no valid

            productCategory.categorySelectList = _db.Category.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()

            });

            productCategory.ApplicationTypeSelecList = _db.ApplicationType.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()

            });

            return View(productCategory);

        }

        //DELETE GETs
        public IActionResult Delete(int? id)
        {

            if ( id == null || id == 0) {

                return NotFound();
            
            }

            //Eager Loading

            Product product = _db.Product.Include(x => x.Category).Include(x => x.ApplicationType).FirstOrDefault( x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Product.Find(id);

            if (obj == null) {

                return NotFound();
            }

            string upload = _webHostEnviaronment.WebRootPath + WC.ImagePath;
            var oldFile = Path.Combine(upload, obj.Image);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }

            _db.Product.Remove(obj);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
