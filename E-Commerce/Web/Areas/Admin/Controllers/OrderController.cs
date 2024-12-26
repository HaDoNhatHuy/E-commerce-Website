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
        [HttpPost]
        [Route("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder(string orderCode, int status)
        {
            var order = await _dataContext.Orders.Where(i => i.OrderCode == orderCode).FirstOrDefaultAsync();
            if (order == null)
            {
                return NotFound();
            }
            order.Status = status;
            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Trạng thái đơn hàng đã được cập nhật." });
            }
            catch (Exception ex)
            {
                //return StatusCode(500, "An error occured while updating the order status");
                return Ok(new { success = false, message = "Đã xảy ra lỗi khi cập nhật trạng thái đơn hàng." });
            }
        }
    }
}
