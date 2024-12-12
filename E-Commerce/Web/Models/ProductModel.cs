using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập tên sản phẩm")]
        public string Picture { get; set; }
        public string Name { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập mô tả sản phẩm")]
        public string Slug { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public Guid BrandId { get; set; }
        public CategoryModel Category { get; set; }
        public BrandModel Brand { get; set; }
    }
}
