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
            var orderInfo = await _dataContext.Orders.Where(i => i.OrderCode == orderCode).ToListAsync();
            ViewBag.OrderInfo = orderInfo;
            var orderDetails = await _dataContext.OrderDetails.Include(i => i.Product).Where(i => i.OrderCode == orderCode).ToListAsync();
            var order = _dataContext.Orders.Where(i => i.OrderCode == orderCode).First();
            ViewBag.Status = order.Status;
            var paymentDate=_dataContext.MomoInfos.FirstOrDefaultAsync(i=>)
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
            _dataContext.Update(order);
            if (status == 1)
            {
                var detailsOrder = await _dataContext.OrderDetails
                    .Include(i => i.Product)
                    .Where(i => i.OrderCode == order.OrderCode)
                    .Select(i => new
                    {
                        i.Quantity,
                        i.Product.Price,
                        i.Product.CapitalPrice
                    }).ToListAsync();
                //lấy data thống kê dựa vào ngày đặt hàng
                var statisticalModel = await _dataContext.Statisticals
                    .FirstOrDefaultAsync(i => i.CreatedDate.Date == order.CreatedDate.Date);
                if (statisticalModel != null)
                {
                    foreach (var item in detailsOrder)
                    {
                        //tồn tại ngày đó thì cộng dồn
                        statisticalModel.Quantity += 1;
                        statisticalModel.Sold += item.Quantity;
                        statisticalModel.Revenue += item.Quantity * item.Price;
                        statisticalModel.Profit += item.Price - item.CapitalPrice;
                    }
                    _dataContext.Update(statisticalModel);
                }
                else
                {
                    int new_quantity = 0;
                    int new_sold = 0;
                    decimal new_profit = 0;
                    foreach (var item in detailsOrder)
                    {
                        new_quantity += 1;
                        new_sold += item.Quantity;
                        new_profit += item.Price - item.CapitalPrice;
                        statisticalModel = new Models.StatisticalChartModel
                        {
                            CreatedDate = order.CreatedDate,
                            Quantity = new_quantity,
                            Sold = new_sold,
                            Profit = new_profit,
                            Revenue = item.Quantity * item.Price,
                        };
                    }
                    _dataContext.Add(statisticalModel);
                }
            }
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
