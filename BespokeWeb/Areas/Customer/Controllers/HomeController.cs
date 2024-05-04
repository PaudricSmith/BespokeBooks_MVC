using BespokeBooks.DataAccess.Repository.IRepository;
using BespokeBooks.Models;
using BespokeBooks.Models.ViewModels;
using BespokeBooks.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BespokeBooksWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //if (claim != null)
            //{
            //    HttpContext.Session.SetInt32(SD.SessionCart,
            //        _unitOfWork.ShoppingCartRepo.GetAll(s => s.ApplicationUserId == claim.Value).Count());
            //}
            
            IEnumerable<Product> productList = _unitOfWork.ProductRepo.GetAll(includeProperties: "Category,ProductImages");
            return View(productList);
        }

        public IActionResult Details(int productId)
        {
            ShoppingCart shoppingCart = new()
            {
                Product = _unitOfWork.ProductRepo.Get(p => p.Id == productId, includeProperties: "Category,ProductImages"),
                Count = 1,
                ProductId = productId
            };

            return View(shoppingCart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCartRepo
                .Get(s => s.ApplicationUserId == userId && s.ProductId == shoppingCart.ProductId);

            if (cartFromDb != null) 
            {
                // Shopping cart exists
                cartFromDb.Count += shoppingCart.Count;
                
                _unitOfWork.ShoppingCartRepo.Update(cartFromDb);
                _unitOfWork.Save();
            }
            else
            {
                // Add cart record
                _unitOfWork.ShoppingCartRepo.Add(shoppingCart);
                _unitOfWork.Save();

                HttpContext.Session.SetInt32(SD.SessionCart, 
                    _unitOfWork.ShoppingCartRepo.GetAll(a => a.ApplicationUserId == userId).Count());
            }

            TempData["success"] = "Cart updated successfully";

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
