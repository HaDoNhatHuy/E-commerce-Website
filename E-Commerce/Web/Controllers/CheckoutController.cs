using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Models;
using Web.Repository;

namespace Web.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        public CheckoutController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}
        public async Task<IActionResult> Checkout()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var orderCode = Guid.NewGuid().ToString();
                var orderItem = new OrderModel();
                orderItem.OrderCode = orderCode;
                orderItem.UserName = userEmail;
                orderItem.Status = 1;
                orderItem.CreatedDate = DateTime.Now;
                _dataContext.Add(orderItem);
                await _dataContext.SaveChangesAsync();
                List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                foreach (var item in cartItems)
                {
                    var orderDetails = new OrderDetails();
                    orderDetails.OrderCode = orderCode;
                    orderDetails.UserName = userEmail;
                    orderDetails.ProductId = item.ProductId;
                    orderDetails.Price = item.Price;
                    orderDetails.Quantity = item.Quantity;
                    _dataContext.Add(orderDetails);
                    await _dataContext.SaveChangesAsync();
                }
                HttpContext.Session.Remove("Cart");
                TempData["success"] = "Checkout thành công, vui lòng chờ duyệt đơn hàng";
                return RedirectToAction("Index", "Cart");
            }
        }
    }
}
