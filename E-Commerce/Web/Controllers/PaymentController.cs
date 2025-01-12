using Microsoft.AspNetCore.Mvc;
using Web.Models.Order;
using Web.Services.Momo;

namespace Web.Controllers
{
    public class PaymentController : Controller
    {
        private IMomoService _momoService;
        public PaymentController(IMomoService momoService)
        {
            _momoService = momoService;
        }
        [HttpPost]
        public async Task<IActionResult> CreatePaymentUrl(OrderInfoModel model)
        {
            var response = await _momoService.CreatePaymentAsync(model);
            return Redirect(response.PayUrl);
        }
    }
}
