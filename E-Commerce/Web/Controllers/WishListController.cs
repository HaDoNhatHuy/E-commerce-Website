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
                return RedirectToAction("Account", "Account");
            }
            var wishList = await _dataContext.WishListProducts.Include(i => i.Product).Where(i => i.UserId == currentUser.Id).ToListAsync();
            return View(wishList);
        }
        //---CÁCH LẤY THỨ HAI---
        //public async Task<IActionResult> Index()
        //{
        //    var wishlist_product = await (from w in _dataContext.WishListProducts
        //                                  join p in _dataContext.Products on w.ProductId equals p.Id
        //                                  join u in _dataContext.Users on w.UserId equals u.Id
        //                                  select new { User = u, Product = p, WishLists = w })
        //                                  .ToListAsync();
        //    return View(wishlist_product);
        //}
        [HttpGet]
        public async Task<IActionResult> Remove(int Id)
        {
            var productWish = _dataContext.WishListProducts.Where(i => i.ProductId == Id).FirstOrDefault();
            if (productWish != null)
            {
                _dataContext.WishListProducts.Remove(productWish);
                await _dataContext.SaveChangesAsync();

                // Cập nhật lại Session
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var wishListSession = await _dataContext.WishListProducts
                        .Where(i => i.UserId == user.Id)
                        .ToListAsync();
                    HttpContext.Session.SetJson("WishList", wishListSession);
                }
                List<WishListModel> list = HttpContext.Session.GetJson<List<WishListModel>>("WishList");
                if (list.Count == 0)
                {
                    HttpContext.Session.Remove("WishList");
                }
                else
                {
                    HttpContext.Session.SetJson("WishList", list);
                }
            }
            TempData["success"] = "Đã xóa sản phẩm yêu thích";
            //return Ok(new { success = true, message = "Đã xóa sản phẩm yêu thích " });
            return RedirectToAction("Index");
        }
    }
}
