using System.Diagnostics;
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

        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _logger = logger;
            _dataContext = context;
        }

        public IActionResult Index()
        {
            var products = _dataContext.Products.Include("Category").Include("Brand").ToList();

            //var banners = _dataContext.Banners.Where(i => i.Status == 1).ToList();
            // Tách danh sách banner thành hai nhóm
            var bannersGroup1 =  _dataContext.Banners.Take(1).ToList();
            var bannersGroup2 =  _dataContext.Banners.Skip(1).Take(1).ToList();

            ViewBag.BannersGroup1 = bannersGroup1;
            ViewBag.BannersGroup2 = bannersGroup2;

            //ViewBag.Banners = banners;

            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
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
