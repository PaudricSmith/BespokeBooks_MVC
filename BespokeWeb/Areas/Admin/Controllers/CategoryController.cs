using BespokeBooks.Models;
using BespokeBooks.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace BespokeBooksWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.CategoryRepo.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The Name cannot match the Display Order");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepo.Add(category);
                _unitOfWork.Save();

                TempData["Success"] = "Category created successfully!";

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

            Category categoryFromDb = _unitOfWork.CategoryRepo.Get(c => c.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepo.Update(category);
                _unitOfWork.Save();

                TempData["Success"] = "Category updated successfully!";

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

            Category? categoryFromDb = _unitOfWork.CategoryRepo.Get(c => c.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? categoryFromDb = _unitOfWork.CategoryRepo.Get(c => c.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }

            _unitOfWork.CategoryRepo.Remove(categoryFromDb);
            _unitOfWork.Save();

            TempData["Success"] = "Category deleted successfully!";

            return RedirectToAction("Index");

        }
    }
}
