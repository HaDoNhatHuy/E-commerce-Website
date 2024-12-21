using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Repository;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        public HomeController(DataContext context)
        {
            _dataContext = context;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
