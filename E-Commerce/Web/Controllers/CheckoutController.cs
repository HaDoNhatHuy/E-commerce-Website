using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Areas.Admin.Repository;
using Web.Models;
using Web.Models.ViewModels;
using Web.Repository;

namespace Web.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IEmailSender _emailSender;
        public CheckoutController(DataContext dataContext, IEmailSender emailSender)
        {
            _dataContext = dataContext;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(i => i.Quantity * i.Price)
            };
            return View(cartVM);
        }
        [HttpPost]
        public async Task<IActionResult> CheckoutSuccess(CartItemViewModel model)
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
                    orderDetails.UserName = model.UserName;
                    orderDetails.Address = model.Address;
                    orderDetails.Phone = model.Phone;
                    orderDetails.Email = model.Email;
                    orderDetails.Note = model.Note;
                    orderDetails.ProductId = item.ProductId;
                    orderDetails.Price = item.Price;
                    orderDetails.Quantity = item.Quantity;
                    _dataContext.Add(orderDetails);
                    await _dataContext.SaveChangesAsync();
                }
                TempData["success"] = "Checkout thành công, vui lòng chờ duyệt đơn hàng";
                HttpContext.Session.Remove("Cart");
                //Send Email when order success
                var receiver = userEmail;
                var subject = "Đặt hàng thành công";
                var message = "Đặt hàng thành công, trải nghiệm dịch vụ nhé";
                await _emailSender.SendEmailAsync(receiver, subject, message);                
                return RedirectToAction("Index", "Cart");
            }
        }
    }
}

