using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    public class AppUserModel : IdentityUser
    {
        public string Occupation { get; set; }
        public string RoleId { get; set; }
        public string Token { get; set; }
    }
}
