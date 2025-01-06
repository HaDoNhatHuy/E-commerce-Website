using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Web.Models;
using Web.Models.ViewModels;
using Web.Repository;

namespace Web.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<AppUserModel> _userManager;
        public CartController(DataContext context, UserManager<AppUserModel> userManager)
        {
            _dataContext = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            Response.Cookies.Delete("CouponTitle");
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Account", "Account");
            }
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            //Nhận phí shipping từ Cookie
            var shippingPriceCookie = Request.Cookies["ShippingPrice"];
            decimal shippingPrice = 0;
            //Nhận Coupon từ Cookie

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


            if (shippingPriceCookie != null)
            {
                var shippingPriceJson = shippingPriceCookie;
                shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
            }

            //discountValue = couponCodeInDB != null ? couponCodeInDB.Discount : 0;
            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(i => i.Quantity * i.Price),
                ShippingFee = shippingPrice,
                CouponCode = couponCode
                //Discount = discountValue,
            };

            return View(cartVM);
        }
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Account", "Account");
            }
            return View("~/Views/Checkout/Index.cshtml");
        }
        public async Task<IActionResult> Add(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.Where(i => i.ProductId == Id).FirstOrDefault();
            if (cartItem == null)
            {
                cart.Add(new CartItemModel(product));
            }
            else
            {
                cartItem.Quantity += 1;
            }
            HttpContext.Session.SetJson("Cart", cart);
            TempData["success"] = "Thêm sản phẩm vào giỏ hàng thành công";
            return Redirect(Request.Headers["Referer"].ToString());//trả về trang hiện tại
        }
        public async Task<IActionResult> Decrease(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.Where(i => i.ProductId == Id).FirstOrDefault();
            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(i => i.ProductId == Id);
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Increase(int Id)
        {
            var product = await _dataContext.Products.Where(i => i.Id == Id).FirstOrDefaultAsync();
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.Where(i => i.ProductId == Id).FirstOrDefault();
            if (cartItem.Quantity >= 1 && product.Quantity > cartItem.Quantity)
            {
                ++cartItem.Quantity;
            }
            else
            {
                cartItem.Quantity = product.Quantity;
                TempData["error"] = "Số lượng sản phẩm trong giỏ hàng đã tối đa ";
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Remove(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            cart.RemoveAll(i => i.ProductId == Id);
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Clear()
        {
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Index");
        }
        //Tính phí ship
        [HttpPost]
        [Route("Cart/GetShipping")]
        public async Task<IActionResult> GetShipping(ShippingModel shippingModel, string quan, string tinh, string phuong)
        {
            var existingShipping = await _dataContext.Shippings.FirstOrDefaultAsync(i => i.Ward == phuong && i.District == quan && i.City == tinh);
            decimal shippingPrice = 0;
            if (existingShipping != null)
            {
                shippingPrice = existingShipping.Price;
            }
            else
            {
                shippingPrice = 50000; //Nếu tìm không thấy thì cho giá là 50k
            }
            var shippingPriceJson = JsonConvert.SerializeObject(shippingPrice);
            try
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                    Secure = true, //using HTTPS
                };
                Response.Cookies.Append("ShippingPrice", shippingPriceJson, cookieOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding shipping price cookie: {ex.Message}");
            }
            return Json(new { shippingPrice, success = true });
        }
        [HttpGet]
        [Route("DeleteShipping")]
        public IActionResult DeleteShipping()
        {
            Response.Cookies.Delete("ShippingPrice");
            //return Json(new { success = true });
            return RedirectToAction("Index", "Cart");
        }
        [HttpPost]
        public async Task<IActionResult> GetCoupon(CouponModel coupon, string coupon_value)
        {
            var validCoupon = await _dataContext.Coupons
                .FirstOrDefaultAsync(i => i.Name == coupon_value);
            string couponTitle = validCoupon?.Name/* + " | " + validCoupon.Description*/;
            if (couponTitle != null)
            {
                TimeSpan remainingTime = validCoupon.DateExpired - DateTime.Now;
                int remainingDays = remainingTime.Days;
                if (remainingDays >= 0)
                {
                    try
                    {
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                            Secure = true,
                            SameSite = SameSiteMode.Strict // Kiểm tra tính tương thích trình duyệt
                        };
                        //TempData["success"] = $"Mã giảm giá '{coupon_value}' đã được áp dụng. Giảm giá {coupon.Discount * 100}%!";
                        Response.Cookies.Append("CouponTitle", couponTitle, cookieOptions);
                        //TempData["success"] = "Đã áp dụng mã giảm giá";
                        return Ok(new { success = true, message = "Đã áp dụng mã giảm giá" });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error adding apply coupon cookie:{ex.Message}");
                        //TempData["error"] = "Áp dụng mã giảm giá thất bại";
                        return Ok(new { success = false, message = "Áp dụng mã giảm giá thất bại" });
                    }
                }
                else
                {
                    //TempData["error"] = $"Mã giảm giá '{coupon_value}' đã hết hạn!";
                    return Ok(new { success = false, message = "Mã giảm giá hết hạn !" });
                }
            }
            else
            {
                //TempData["error"] = $"Mã giảm giá '{coupon_value}' không tồn tại!";
                return Ok(new { success = false, message = "Mã giảm giá không tồn tại !" });
            }
        }
    }
}

