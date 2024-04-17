using BespokeBooks.Models;
using BespokeBooks.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using BespokeBooks.Models.ViewModels;
using BespokeBooks.Utility;
using Microsoft.AspNetCore.Authorization;

namespace BespokeBooksWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            List<Company> companyList = _unitOfWork.CompanyRepo.GetAll().ToList();

            return View(companyList);
        }

        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                // Create
                return View(new Company());
            }
            else
            {
                // Update
                Company company = _unitOfWork.CompanyRepo.Get(c => c.Id == id);
                return View(company);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {   
                if (company.Id == 0)
                {
                    _unitOfWork.CompanyRepo.Add(company);
                }
                else
                {
                    _unitOfWork.CompanyRepo.Update(company);
                }

                _unitOfWork.Save();

                TempData["Success"] = "Company created successfully!";

                return RedirectToAction("Index");
            }
            else
            {
                return View(company);
            }
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> companyLIst = _unitOfWork.CompanyRepo.GetAll().ToList();
            return Json(new { data = companyLIst });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var companyToBeDeleted = _unitOfWork.CompanyRepo.Get(c => c.Id == id);
            if (companyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting!" });
            }

            _unitOfWork.CompanyRepo.Remove(companyToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Company deleted successfully!" });
        }

        #endregion
    }
}
