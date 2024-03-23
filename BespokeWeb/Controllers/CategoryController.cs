using BespokeBooks.Models;
using BespokeBooks.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace BespokeBooksWeb.Controllers
{
    [Controller]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepo;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepo = categoryRepository;
        }


        public IActionResult Index()
        {
            List<Category> objCategoryList = _categoryRepo.GetAll().ToList();
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
                _categoryRepo.Add(category);
                _categoryRepo.Save();

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

            Category categoryFromDb = _categoryRepo.Get(c => c.Id == id);
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
                _categoryRepo.Update(category);
                _categoryRepo.Save();

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

            Category? categoryFromDb = _categoryRepo.Get(c => c.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? categoryFromDb = _categoryRepo.Get(c => c.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }

            _categoryRepo.Remove(categoryFromDb);
            _categoryRepo.Save();

            TempData["Success"] = "Category deleted successfully!";

            return RedirectToAction("Index");
            
        }
    }
}
