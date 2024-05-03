using BespokeBooks.Models;
using BespokeBooks.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using BespokeBooks.Models.ViewModels;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using BespokeBooks.Utility;
using Microsoft.AspNetCore.Authorization;

namespace BespokeBooksWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }


        public IActionResult Index()
        {
            List<Product> productList = _unitOfWork.ProductRepo.GetAll(includeProperties : "Category").ToList();

            return View(productList);
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
                productVM.Product = _unitOfWork.ProductRepo.Get(p => p.Id == id, includeProperties: "ProductImages");
                return View(productVM);
            }

        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile>? files)
        {
            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0)
                    _unitOfWork.ProductRepo.Add(productVM.Product);
                else
                    _unitOfWork.ProductRepo.Update(productVM.Product);

                _unitOfWork.Save();


                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {
                    foreach (IFormFile file in files) 
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + productVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);
                    
                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create)) 
                            file.CopyTo(fileStream);

                        ProductImage productImage = new ProductImage()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = productVM.Product.Id,
                        };

                        if (productVM.Product.ProductImages == null)
                            productVM.Product.ProductImages = new List<ProductImage>();

                        productVM.Product.ProductImages.Add(productImage);

                    }

                    _unitOfWork.ProductRepo.Update(productVM.Product);
                    _unitOfWork.Save();
                }

                TempData["Success"] = "Product created/updated successfully!";

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

        public IActionResult DeleteImage(int imageId)
        {
            var imageToBeDeleted = _unitOfWork.ProductImageRepo.Get(i => i.Id == imageId);
            int productId = imageToBeDeleted.ProductId;

            if (imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageToBeDeleted.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }

                _unitOfWork.ProductImageRepo.Remove(imageToBeDeleted);
                _unitOfWork.Save();

                TempData["success"] = "Deleted Successfully!";
            }

            return RedirectToAction(nameof(Upsert), new { id = productId });
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll() 
        {
            List<Product> productList = _unitOfWork.ProductRepo.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = productList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.ProductRepo.Get(c => c.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting!" });
            }

            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }

                Directory.Delete(finalPath);
            }

            _unitOfWork.ProductRepo.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Product deleted successfully!" });
        }

        #endregion
    }
}
