using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using Web.Areas.Admin.Repository;
using Web.Models;
using Web.Models.Order;
using Web.Models.ViewModels;
using Web.Repository;
using Web.Services.Momo;

namespace Web.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IEmailSender _emailSender;
        private IMomoService _momoService;
        public CheckoutController(DataContext dataContext, IEmailSender emailSender, IMomoService momoService)
        {
            _dataContext = dataContext;
            _emailSender = emailSender;
            _momoService = momoService;
        }

        public async Task<IActionResult> Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            //Nhận phí shipping từ Cookie
            var shippingPriceCookie = Request.Cookies["ShippingPrice"];
            var couponCode = Request.Cookies["CouponTitle"];
            if (couponCode == null)
            {
                ViewBag.CouponCode = 0;
            }
            else
            {
                var couponCodeInDB = await _dataContext.Coupons.Where(i => i.Name.Equals(couponCode)).FirstOrDefaultAsync();
                ViewBag.CouponCode = couponCodeInDB.Discount; // Gán giá trị giảm giá hoặc 0 nếu không tìm thấy
            }
            decimal shippingPrice = 0;
            if (shippingPriceCookie != null)
            {
                var shippingPriceJson = shippingPriceCookie;
                shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
            }
            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(i => i.Quantity * i.Price),
                ShippingFee = shippingPrice,
                CouponCode = couponCode
            };
            return View(cartVM);
        }
        [HttpPost]
        public async Task<IActionResult> CheckoutSuccess(CartItemViewModel model, OrderInfoModel orderInfoModel, string PaymentMethod)
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

                //Nhận phí shipping từ Cookie
                var shippingPriceCookie = Request.Cookies["ShippingPrice"];
                decimal shippingPrice = 0;
                if (shippingPriceCookie != null)
                {
                    var shippingPriceJson = shippingPriceCookie;
                    shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
                }
                orderItem.ShippingFee = shippingPrice;

                orderItem.UserName = userEmail;
                orderItem.Status = 0;
                orderItem.CreatedDate = DateTime.Now;
                orderItem.PaymentMethod = PaymentMethod;
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
                    //update product quantity
                    var product = await _dataContext.Products.Where(p => p.Id == item.ProductId).FirstOrDefaultAsync();
                    product.Quantity -= item.Quantity;
                    product.Sold += item.Quantity;
                    _dataContext.Products.Update(product);
                    _dataContext.Add(orderDetails);
                    await _dataContext.SaveChangesAsync();
                }
                if (PaymentMethod == "Momo")
                {
                    var response = await _momoService.CreatePaymentAsync(orderInfoModel);
                    TempData["success"] = "Checkout thành công, vui lòng chờ duyệt đơn hàng";
                    HttpContext.Session.Remove("Cart");
                    //Send Email when order success
                    var receiver = userEmail;
                    var subject = "Đặt hàng thành công";
                    var message = "Đặt hàng thành công, trải nghiệm dịch vụ nhé";
                    await _emailSender.SendEmailAsync(receiver, subject, message);

                    return Redirect(response.PayUrl);
                }
                else
                {
                    TempData["success"] = "Checkout thành công, vui lòng chờ duyệt đơn hàng";
                    //Response.Cookies.Delete("CouponTitle");
                    HttpContext.Session.Remove("Cart");
                    //Send Email when order success
                    var receiver = userEmail;
                    var subject = "Đặt hàng thành công";
                    var message = "Đặt hàng thành công, trải nghiệm dịch vụ nhé";
                    await _emailSender.SendEmailAsync(receiver, subject, message);
                    return RedirectToAction("History", "Account");
                }
            }
        }
        [HttpGet]
        public async Task<IActionResult> PaymentCallBack(MomoInfoModel momoModel)
        {
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            var requestQuery = HttpContext.Request.Query;
            if (requestQuery["resultCode"] != 0) //Giao dịch không thành công
            {
                var newMomoInfo = new MomoInfoModel
                {
                    Id = Guid.NewGuid().ToString(),
                    OrderId = requestQuery["orderId"],
                    FullName = User.FindFirstValue(ClaimTypes.Email),
                    Amount = decimal.Parse(requestQuery["amount"]),
                    OrderInfo = requestQuery["orderInfo"],
                    DatePaid = DateTime.Now,
                };
                _dataContext.Add(newMomoInfo);
                await _dataContext.SaveChangesAsync();
            }
            else
            {
                TempData["error"] = "Giao dịch không thành công";
                return RedirectToAction("Index", "Cart");
            }
            return View(response);
        }
    }
}

