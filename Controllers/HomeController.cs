using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rocky.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rocky.Data;
using Rocky.Models.ViewModels;
using Rocky.Utility;

namespace Rocky.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            HomeVM homeVm = new HomeVM()
            {
                Products = _dbContext.Product.Include(p => p.Category).Include(p => p.ApplicationType),
                Categories = _dbContext.Category
            };
            return View(homeVm);
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

        public IActionResult Details(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if(HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart)!=null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count()>0){
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            
            DetailsVM detailsVM = new DetailsVM{
                Product = _dbContext.Product.Include(u => u.Category).Include(u => u.ApplicationType).FirstOrDefault(u => u.Id == id),
                ExistInCart = false
            };
            foreach(var item in shoppingCartList){
                if(item.ProductId == id){
                    detailsVM.ExistInCart = true;
                }
            }
            return View(detailsVM);
        }

        [HttpPost]
        [ActionName("Details")]
        public IActionResult DetailsPost(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if(HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart)!=null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count()>0){
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            shoppingCartList.Add(new ShoppingCart{
                ProductId = id
            });
            HttpContext.Session.Set(WC.SessionCart,shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int id){
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if(HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart)!=null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count()>0){
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            var itemToRemove = shoppingCartList.FirstOrDefault(u => u.ProductId == id);
            if(itemToRemove != null){
                shoppingCartList.Remove(itemToRemove);
            }
            HttpContext.Session.Set(WC.SessionCart,shoppingCartList);
            return RedirectToAction(nameof(Index));
        }
    }
}
