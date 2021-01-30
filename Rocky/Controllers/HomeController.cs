using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
using Rocky.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            HomeViewModel homeView = new HomeViewModel()
            {
                products = _db.Product.Include(u => u.Category).Include(u => u.ApplicationType),
                categories = _db.Category

            };
            return View(homeView);
        }

        public IActionResult Details(int id)
        {
            List<ShoppingCart> shoppings = new List<ShoppingCart>();

            if (HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppings = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }


            DetailsViewModel detailsView = new DetailsViewModel()
            {
                product = _db.Product.Include(u => u.Category).Include(u => u.ApplicationType)
                .Where(x => x.Id == id).FirstOrDefault(),

                ExistsInCart = false
            };

            foreach (var item in shoppings)
            {
                if (item.ProductId == id)
                {
                    detailsView.ExistsInCart = true;
                }

            }


            return View(detailsView);
        }


        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart> shoppings = new List<ShoppingCart>();

            if (HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppings = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            var itemToRemove = shoppings.SingleOrDefault(x => x.ProductId == id);
            if (itemToRemove != null)
            {
                shoppings.Remove(itemToRemove);
            }

            HttpContext.Session.Set(WC.SessionCart, shoppings);

            return RedirectToAction(nameof(Index));



        }




        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id)
        {
            List<ShoppingCart> shoppings = new List<ShoppingCart>();

            if (HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart).Count() > 0 )
            {
                shoppings = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            shoppings.Add(new ShoppingCart { ProductId = id });

            HttpContext.Session.Set(WC.SessionCart, shoppings);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
