using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    public class CompareModel
    {
        [Key]
        public int Id { get; set; }        
        public string UserId { get; set; }
        [ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public ProductModel Product { get; set; }

    }
}
