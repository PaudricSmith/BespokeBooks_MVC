using BespokeBooks.Models;
using BespokeBooks.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BespokeBooks.Utility;
using BespokeBooks.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using BespokeBooks.DataAccess.Repository;
using BespokeBooks.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;

namespace BespokeBooksWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string userId)
        {
            string roleId = _dbContext.UserRoles.FirstOrDefault(a => a.UserId == userId).RoleId;

            RoleManagementVM roleManagementVM = new RoleManagementVM()
            {
                ApplicationUser = _dbContext.ApplicationUsers.Include(a => a.Company).FirstOrDefault(u => u.Id == userId),

                RoleList = _dbContext.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _dbContext.Companies.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            roleManagementVM.ApplicationUser.Role = _dbContext.Roles.FirstOrDefault(r => r.Id == roleId).Name;

            return View(roleManagementVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {
            string roleId = _dbContext.UserRoles.FirstOrDefault(a => a.UserId == roleManagementVM.ApplicationUser.Id).RoleId;
            string oldRole = _dbContext.Roles.FirstOrDefault(r => r.Id == roleId).Name;

            ApplicationUser applicationUser = _dbContext.ApplicationUsers.FirstOrDefault(a => a.Id == roleManagementVM.ApplicationUser.Id);


            if (!(roleManagementVM.ApplicationUser.Role == oldRole))
            {
                // A role was updated
                if (roleManagementVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = roleManagementVM.ApplicationUser.CompanyId;
                }
                if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }

                _dbContext.SaveChanges();

                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagementVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            else
            {
                if (oldRole == SD.Role_Company && applicationUser.CompanyId != roleManagementVM.ApplicationUser.CompanyId)
                {
                    applicationUser.CompanyId = roleManagementVM.ApplicationUser.CompanyId;
                    _dbContext.ApplicationUsers.Update(applicationUser);
                    _dbContext.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> userList = _dbContext.ApplicationUsers.Include(a => a.Company).ToList();
            var userRoles = _dbContext.UserRoles.ToList();
            var roles = _dbContext.Roles.ToList();

            foreach (var user in userList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(r => r.Id == roleId).Name;

                if (user.Company == null)
                {
                    user.Company = new Company() 
                    { 
                        Name = "" 
                    };
                }
            }

            return Json(new { data = userList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var objFromDb = _dbContext.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking!" });
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                // User is currently locked out and we need to unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            _dbContext.SaveChanges();

            return Json(new { success = true, message = "Operation successfully!" });
        }

        #endregion

    }
}
