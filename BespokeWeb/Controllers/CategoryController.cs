using BespokeWeb.Data;
using BespokeWeb.Models;
using Microsoft.AspNetCore.Mvc;

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
    }
}
