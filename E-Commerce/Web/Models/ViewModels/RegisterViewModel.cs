using System.ComponentModel.DataAnnotations;

namespace Web.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập UserName")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Email"), EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }
    }
}
