using Microsoft.AspNetCore.Mvc;
using Web.Repository;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
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
    }
}
