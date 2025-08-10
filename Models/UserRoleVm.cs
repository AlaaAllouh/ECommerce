using Microsoft.AspNetCore.Identity;

namespace ECommerce.Models
{
    public class UserRoleVm
    {
        public UserRoleVm()
        {
            userRoles = new List<string>();
        }
        public IdentityUser User { get; set; } 
        public List<string>userRoles { get; set; }





    }
}
