using BespokeWeb.Data;
using BespokeWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BespokeWeb.Controllers
{
    [Controller]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public IActionResult Index()
        {
            List<Category> objCategoryList = _dbContext.Categories.ToList();
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
                _dbContext.Categories.Add(category);
                _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            return View();
        }
    }
}
