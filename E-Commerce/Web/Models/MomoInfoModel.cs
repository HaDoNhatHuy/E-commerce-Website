using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    public class MomoInfoModel
    {
        [Key]
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string OrderInfo { get; set; }
        public string FullName { get; set; }
        public decimal Amount { get; set; }
        public DateTime DatePaid { get; set; }       
    }
}
