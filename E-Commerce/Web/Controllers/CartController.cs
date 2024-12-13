using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Models.ViewModels;
using Web.Repository;

namespace Web.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        public CartController(DataContext context)
        {
            _dataContext = context;
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
        public IActionResult Checkout()
        {
            return View("~/Views/Checkout/Index.cshtml");
        }
    }
}
