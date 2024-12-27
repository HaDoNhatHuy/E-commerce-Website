using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Web.Repository.Validation;

namespace Web.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        //[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập tên sản phẩm")]
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        public string Name { get; set; }
        //[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập mô tả sản phẩm")]
        [Required(ErrorMessage = "Vui lòng nhập mô tả sản phẩm")]
        public string Description { get; set; }
        public string Slug { get; set; }
        [Range(0.01, double.MaxValue)]
        [Column(TypeName = "decimal(12,2)")]
        [Required(ErrorMessage = "Vui lòng nhập giá của sản phẩm")]
        public decimal Price { get; set; }
        [Required, Range(1, int.MaxValue, ErrorMessage = "Chọn một danh mục")]
        //[Required(ErrorMessage = "Vui lòng chọn một danh mục")]
        public int CategoryId { get; set; }
        [Required, Range(1, int.MaxValue, ErrorMessage = "Chọn một thương hiệu")]
        //[Required(ErrorMessage = "Vui lòng chọn một thương hiệu")]
        public int BrandId { get; set; }
        public CategoryModel Category { get; set; }
        public BrandModel Brand { get; set; }
        public string Picture { get; set; } = "noimage.jpg";
        [NotMapped]
        [FileExtension]
        [Required(ErrorMessage = "Vui lòng chọn ảnh sản phẩm")]
        public IFormFile PictureUpload { get; set; }
    }
}
