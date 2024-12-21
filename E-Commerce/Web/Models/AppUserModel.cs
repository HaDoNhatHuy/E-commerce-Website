using Microsoft.AspNetCore.Identity;

namespace Web.Models
{
    public class AppUserModel : IdentityUser
    {
        public string Occupation { get; set; }
    }
}
