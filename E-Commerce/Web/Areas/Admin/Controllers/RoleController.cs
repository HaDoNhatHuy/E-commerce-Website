using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Repository;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Route("Admin/Role")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleController(DataContext dataContext, RoleManager<IdentityRole> roleManager)
        {
            _dataContext = dataContext;
            _roleManager = roleManager;
        }
        //[Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Roles.OrderByDescending(i => i.Id).ToListAsync());
        }
        //[Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        //[Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            //Check xem Role có tồn tại hay không
            if (!_roleManager.RoleExistsAsync(role.Name).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(role.Name)).GetAwaiter().GetResult();
            }
            return Redirect("Index");
        }
        [HttpGet]
        //[Route("Delete")]
        public async Task<IActionResult> Delete(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            var role = await _roleManager.FindByIdAsync(Id);
            if (role == null)
            {
                return NotFound();
            }
            try
            {
                await _roleManager.DeleteAsync(role);
                TempData["success"] = "Role delected successfully";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while delecting the role");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        //[Route("Edit")]
        public async Task<IActionResult> Edit(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            var role = await _roleManager.FindByIdAsync(Id);
            return View(role);
        }
        [HttpPost]
        //[Route("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Id, IdentityRole roleModel)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(Id);
                if (role == null)
                {
                    return NotFound();
                }
                role.Name = roleModel.Name;
                try
                {
                    await _roleManager.UpdateAsync(role);
                    TempData["success"] = "Role updated successfully";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while updating the role");
                }
            }
            return View(roleModel ?? new IdentityRole(Id = Id));
        }
    }
}
