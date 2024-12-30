using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Web.Repository.Validation;

namespace Web.Models
{
    public class BannerModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên banner")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mô tả banner")]
        public string Description { get; set; }
        public int Status { get; set; }
        public string Image { get; set; }
        [NotMapped]
        [FileExtension]
        public IFormFile ImageUpload { get; set; }

    }
}
