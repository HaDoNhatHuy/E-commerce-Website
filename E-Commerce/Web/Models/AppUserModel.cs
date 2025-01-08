using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    public class AppUserModel : IdentityUser
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Occupation { get; set; }
        public string RoleId { get; set; }
        public string Token { get; set; }
        [NotMapped]
        public string NewPassword { get; set; }
        [NotMapped]
        public string ConfirmPassword { get; set; }
    }
}
