using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    public class RatingModel
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public ProductModel Product { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập đánh giá sản phẩm")]
        public string Comment { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập email")]
        public string Email { get; set; }
        public int Rating { get; set; }
        public string Image { get; set; }
        [NotMapped]
        public IFormFile ImageUpload { get; set; }
    }
}
