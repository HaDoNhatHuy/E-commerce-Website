using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Repository;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _environment;
        public DashboardController(DataContext context, IWebHostEnvironment environment)
        {
            _dataContext = context;
            _environment = environment;
        }
        public IActionResult Index()
        {
            var count_product = _dataContext.Products.Count();
            var count_order = _dataContext.Orders.Count();
            var count_category = _dataContext.Categories.Count();
            var count_user = _dataContext.Users.Count();
            ViewBag.CountProduct = count_product;
            ViewBag.CountOrder = count_order;
            ViewBag.CountCategory = count_category;
            ViewBag.CountUser = count_user;
            return View();
        }
        [HttpPost]
        //[Route("GetChartData")]
        public IActionResult GetChartData()
        {
            var data = _dataContext.Statisticals.Select(s => new
            {
                date = s.CreatedDate.ToString("dd-MM-yyyy"),
                quantity = s.Quantity,
                sold = s.Sold,
                revenue = s.Revenue,
                profit = s.Profit,
            }).ToList();
            return Json(data);
        }
        [HttpPost]
        //[Route("GetChartDataBySelect")]
        public IActionResult GetChartDataBySelect(DateTime startDate, DateTime endDate)
        {
            var data = _dataContext.Statisticals
                .Where(s => s.CreatedDate >= startDate && s.CreatedDate <= endDate)
                .Select(s => new
                {
                    date = s.CreatedDate.ToString("dd-MM-yyyy"),
                    quantity = s.Quantity,
                    sold = s.Sold,
                    revenue = s.Revenue,
                    profit = s.Profit,
                }).ToList();
            return Json(data);
        }
        [HttpPost]
        //[Route("FilterData")]
        public IActionResult FilterData(DateTime? fromDate, DateTime? toDate)
        {
            var query = _dataContext.Statisticals.AsQueryable();
            if (fromDate.HasValue)
            {
                query = query.Where(s => s.CreatedDate >= fromDate);
            }
            if (toDate.HasValue)
            {
                query = query.Where(s => s.CreatedDate <= toDate);
            }
            var data = query
                .Select(s => new
                {
                    date = s.CreatedDate.ToString("dd-MM-yyyy"),
                    quantity = s.Quantity,
                    sold = s.Sold,
                    revenue = s.Revenue,
                    profit = s.Profit,
                }).ToList();
            return Json(data);
        }
    }
}
