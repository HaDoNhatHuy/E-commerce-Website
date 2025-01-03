using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Models.ViewModels;
using Web.Repository;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUserModel> _userManager;
        public HomeController(ILogger<HomeController> logger, DataContext context, UserManager<AppUserModel> userManager)
        {
            _logger = logger;
            _dataContext = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var products = _dataContext.Products.Include("Category").Include("Brand").ToList();

            //var banners = _dataContext.Banners.Where(i => i.Status == 1).ToList();
            // Tách danh sách banner thành hai nhóm
            var bannersGroup1 = _dataContext.Banners.Take(1).ToList();
            var bannersGroup2 = _dataContext.Banners.Skip(1).Take(1).ToList();

            ViewBag.BannersGroup1 = bannersGroup1;
            ViewBag.BannersGroup2 = bannersGroup2;

            //ViewBag.Banners = banners;

            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult> Contact()
        {
            var contact = await _dataContext.Contacts.FirstOrDefaultAsync();
            return View(contact);
        }
        public async Task<IActionResult> AddWishList(int Id, WishListModel wishList)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(new { success = false, message = "User is not authenticated" });
            }
            var existingProductWish = await _dataContext.WishListProducts.FirstOrDefaultAsync(i => i.ProductId == Id && i.UserId == user.Id);
            if (existingProductWish != null)
            {
                //TempData["error"] = "Sản phẩm đã có trong danh sách yêu thích";
                return BadRequest(new { success = false, message = "Sản phẩm đã có trong danh sách yêu thích" });
            }
            wishList.ProductId = Id;
            wishList.UserId = user.Id;
            // Đảm bảo Id không bị gán giá trị cụ thể
            wishList.Id = 0;
            try
            {
                _dataContext.WishListProducts.Add(wishList);
                await _dataContext.SaveChangesAsync();
                var wishListSession = await _dataContext.WishListProducts
                    .Where(i => i.UserId == user.Id)
                    .Include(i => i.Product)
                    .ToListAsync();
                HttpContext.Session.SetJson("WishList", wishListSession);
                return Ok(new { success = true, message = "Thêm vào yêu thích thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occured while adding to wishlist table");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statuscode)
        {
            if (statuscode == 404)
            {
                return View("NotFound");
            }
            else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
}
