using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Repository;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ShippingController : Controller
    {
        private readonly DataContext _dataContext;
        public ShippingController(DataContext context)
        {
            _dataContext = context;
        }
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var shippingList = await _dataContext.Shippings.ToListAsync();
            ViewBag.ShippingList = shippingList;
            return View();
        }
        [HttpPost]
        [Route("StoreShipping")]
        public async Task<IActionResult> StoreShipping(ShippingModel shippingModel, string tinh, string quan, string phuong, decimal price)
        {
            shippingModel.City = tinh;
            shippingModel.District = quan;
            shippingModel.Ward = phuong;
            shippingModel.Price = price;
            try
            {
                var existingShipping = await _dataContext.Shippings.AnyAsync(i => i.Ward == phuong && i.District == quan && i.City == tinh);
                if (existingShipping)
                {
                    return Ok(new { duplicate = true, message = "Dữ liệu bị trùng lặp!" });
                }
                _dataContext.Shippings.Add(shippingModel);
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Thêm giá vận chuyển thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while adding shipping.");
            }
        }
        public async Task<IActionResult> Delete(int Id)
        {
            var shipping = await _dataContext.Shippings.FindAsync(Id);
            _dataContext.Shippings.Remove(shipping);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Đã xóa phí vận chuyển";
            return RedirectToAction("Index","Shipping");
        }
    }
}
