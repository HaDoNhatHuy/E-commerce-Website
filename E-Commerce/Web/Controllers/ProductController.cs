using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Repository;

namespace Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        public ProductController(DataContext context)
        {
            _dataContext = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(int Id)
        {
            if (Id == null)
                return RedirectToAction("Index");
            var productById = _dataContext.Products.Include("Category").Include("Brand").Where(i => i.Id == Id).FirstOrDefault();
            //var productById = await _dataContext.Products.Where(p => p.Id == Id).FirstOrDefaultAsync();

            //Related Product
            var relatedProducts = await _dataContext.Products
                .Where(i => i.CategoryId == productById.CategoryId && i.Id != productById.Id)
                .Take(4)
                .ToListAsync();
            ViewBag.RelatedProducts = relatedProducts;
            return View(productById);
        }
        [HttpPost]
        public async Task<IActionResult> Search(string searchItem)
        {
            var products = await _dataContext.Products
                .Where(i => i.Name.Contains(searchItem) || i.Description.Contains(searchItem))
                .ToListAsync();

            ViewBag.Keyword = searchItem;
            return View(products);
        }
    }
}
