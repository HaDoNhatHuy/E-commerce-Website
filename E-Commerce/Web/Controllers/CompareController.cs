using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Repository;

namespace Web.Controllers
{
    public class CompareController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<AppUserModel> _userManager;
        public CompareController(DataContext dataContext, UserManager<AppUserModel> userManager)
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
            var compare_product = await _dataContext.CompareProducts.Include(i => i.Product).Where(i => i.UserId == currentUser.Id).ToListAsync();
            return View(compare_product);
        }
        //---CÁCH LẤY THỨ HAI---
        //public async Task<IActionResult> Index()
        //{
        //    var compare_product = await (from c in _dataContext.CompareProducts
        //                                  join p in _dataContext.Products on c.ProductId equals p.Id
        //                                  join u in _dataContext.Users on c.UserId equals u.Id
        //                                  select new { User = u, Product = p, Compares = c })
        //                                  .ToListAsync();
        //    return View(compare_product);
        //}
        [HttpGet]
        public async Task<IActionResult> Remove(int Id)
        {
            var productCompare = _dataContext.CompareProducts.Where(i => i.ProductId == Id).FirstOrDefault();
            if (productCompare != null)
            {
                _dataContext.CompareProducts.Remove(productCompare);
                await _dataContext.SaveChangesAsync();

                // Cập nhật lại Session
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var compareSession = await _dataContext.CompareProducts
                        .Where(i => i.UserId == user.Id)
                        .ToListAsync();
                    HttpContext.Session.SetJson("Compare", compareSession);
                }
                List<CompareModel> list = HttpContext.Session.GetJson<List<CompareModel>>("Compare");
                if (list.Count == 0)
                {
                    HttpContext.Session.Remove("Compare");
                }
                else
                {
                    HttpContext.Session.SetJson("Compare", list);
                }
            }
            TempData["success"] = "Đã xóa sản phẩm so sánh";
            //return Ok(new { success = true, message = "Đã xóa sản phẩm yêu thích " });
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Clear()
        {
            foreach (var item in _dataContext.CompareProducts)
            {
                _dataContext.CompareProducts.Remove(item);
            }
            await _dataContext.SaveChangesAsync();
            HttpContext.Session.Remove("Compare");
            TempData["success"] = "Đã xóa toàn bộ sản phẩm so sánh";
            return Redirect(Request.Headers["Referer"]);
        }
    }
}
