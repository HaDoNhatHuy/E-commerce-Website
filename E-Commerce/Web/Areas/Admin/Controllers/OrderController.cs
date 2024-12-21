using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Repository;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;
        public OrderController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Orders.OrderByDescending(i => i.Id).ToListAsync());
        }
        public async Task<IActionResult> ViewOrder(string orderCode)
        {
            var orderDetails = await _dataContext.OrderDetails.Include(i => i.Product).Where(i => i.OrderCode == orderCode).ToListAsync();
            return View(orderDetails);
        }
    }
}
