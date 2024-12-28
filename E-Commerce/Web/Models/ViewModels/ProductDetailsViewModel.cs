using System.ComponentModel.DataAnnotations;

namespace Web.Models.ViewModels
{
    public class ProductDetailsViewModel
    {
        public ProductModel ProductDetails { get; set; }
        //public RatingModel RatingDetails { get; set; }
        public List<RatingModel> ListComments { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập đánh giá sản phẩm")]
        public string Comment { get; set; }
    }
}
