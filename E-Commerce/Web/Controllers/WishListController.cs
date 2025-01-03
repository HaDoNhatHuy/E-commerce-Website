using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Repository;

namespace Web.Controllers
{
    public class WishListController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<AppUserModel> _userManager;
        public WishListController(DataContext dataContext, UserManager<AppUserModel> userManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }
            var wishList = await _dataContext.WishListProducts.Include(i => i.Product).Where(i => i.UserId == currentUser.Id).ToListAsync();
            return View(wishList);
        }
        [HttpGet]
        public async Task<IActionResult> Remove(int Id)
        {
            var productWish = _dataContext.WishListProducts.Where(i => i.ProductId == Id).FirstOrDefault();
            if (productWish != null)
            {
                _dataContext.WishListProducts.Remove(productWish);
                await _dataContext.SaveChangesAsync();
            }
            TempData["success"] = "Đã xóa sản phẩm yêu thích";
            //return Ok(new { success = true, message = "Đã xóa sản phẩm yêu thích " });
            return RedirectToAction("Index");
        }
    }
}
