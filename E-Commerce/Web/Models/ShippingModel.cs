using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class ShippingModel
    {
        [Key]
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string City { get; set; }
    }
}
