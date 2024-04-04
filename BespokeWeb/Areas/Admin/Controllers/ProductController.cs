using BespokeBooks.Models;
using BespokeBooks.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using BespokeBooks.Models.ViewModels;

namespace BespokeBooksWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.ProductRepo.GetAll().ToList();

            return View(objProductList);
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.CategoryRepo
                .GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),

                Product = new Product()
            };

            if (id == null || id == 0)
            {
                // Create
                return View(productVM);
            }
            else
            {
                // Update
                productVM.Product = _unitOfWork.ProductRepo.Get(p => p.Id == id);
                return View(productVM);
            }

        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepo.Add(productVM.Product);
                _unitOfWork.Save();

                TempData["Success"] = "Product created successfully!";

                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.CategoryRepo
                .GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });

                return View(productVM);
            }
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product? productFromDb = _unitOfWork.ProductRepo.Get(c => c.Id == id);
            if (productFromDb == null)
            {
                return NotFound();
            }

            return View(productFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Product? productFromDb = _unitOfWork.ProductRepo.Get(c => c.Id == id);
            if (productFromDb == null)
            {
                return NotFound();
            }

            _unitOfWork.ProductRepo.Remove(productFromDb);
            _unitOfWork.Save();

            TempData["Success"] = "Product deleted successfully!";

            return RedirectToAction("Index");

        }
    }
}
