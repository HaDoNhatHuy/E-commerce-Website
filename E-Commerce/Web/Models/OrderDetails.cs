using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string OrderCode { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }
        public OrderModel Order { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
