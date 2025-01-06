using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Repository;

namespace Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index(string Slug = "", string sort_by = "", string startprice = "", string endprice = "")
        {
            CategoryModel category = _dataContext.Categories.Where(i => i.Slug == Slug).FirstOrDefault();
            if (category == null)
                return RedirectToAction("Index");
            ViewBag.Slug = Slug;
            //lấy tất cả sản phẩm
            IQueryable<ProductModel> productsByCategory = _dataContext.Products.Where(i => i.CategoryId == category.Id);
            var count = await productsByCategory.CountAsync();
            if (count > 0)
            {
                if (sort_by == "price_increase")
                {
                    productsByCategory = productsByCategory.OrderBy(i => i.Price);
                }
                else if (sort_by == "price_decrease")
                {
                    productsByCategory = productsByCategory.OrderByDescending(i => i.Price);
                }
                else if (sort_by == "price_newest")
                {
                    productsByCategory = productsByCategory.OrderByDescending(i => i.Id);
                }
                else if (sort_by == "oldest_newest")
                {
                    productsByCategory = productsByCategory.OrderBy(i => i.Id);
                }
                else if (startprice != "" && endprice != "")
                {
                    decimal startPriceValue;
                    decimal endPriceValue;
                    if (decimal.TryParse(startprice, out startPriceValue) && decimal.TryParse(endprice, out endPriceValue))
                    {
                        productsByCategory = productsByCategory.Where(i => i.Price >= startPriceValue && i.Price <= endPriceValue);
                    }
                    else
                    {
                        productsByCategory = productsByCategory.OrderByDescending(i => i.Id);
                    }
                }
                else
                {
                    productsByCategory = productsByCategory.OrderByDescending(i => i.Id);
                }
            }
            return View(await productsByCategory.ToListAsync());
        }
    }
}
