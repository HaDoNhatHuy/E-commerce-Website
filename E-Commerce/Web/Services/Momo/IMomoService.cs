using Web.Models.Momo;
using Web.Models.Order;

namespace Web.Services.Momo
{
    public interface IMomoService 
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfoModel model);
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
}
