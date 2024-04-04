using BespokeBooks.Models;
using BespokeBooks.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepo.Add(product);
                _unitOfWork.Save();

                TempData["Success"] = "Product created successfully!";

                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product productFromDb = _unitOfWork.ProductRepo.Get(p => p.Id == id);
            if (productFromDb == null)
            {
                return NotFound();
            }

            return View(productFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepo.Update(product);
                _unitOfWork.Save();

                TempData["Success"] = "Product updated successfully!";

                return RedirectToAction("Index");
            }

            return View();
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
