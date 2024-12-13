using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Repository;

namespace Web.Controllers
{
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index(string Slug = "")
        {
            BrandModel brand = _dataContext.Brands.Where(i => i.Slug == Slug).FirstOrDefault();
            if (brand == null)
                return RedirectToAction("Index");
            var productByBrand = _dataContext.Products.Where(i => i.BrandId == brand.Id);
            return View(await productByBrand.OrderByDescending(i => i.Id).ToListAsync());
        }
    }
}
