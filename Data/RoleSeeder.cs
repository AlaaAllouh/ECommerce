using Microsoft.AspNetCore.Identity;

namespace ECommerce.Data
{
    public class RoleSeeder
    {
        public static async Task seedAsync(RoleManager<IdentityRole>rolemanager)
        {
            string[] roles = { "Admin", "SuperAdmin", "SalesManager" };
            foreach (var role in roles)
            {
                if(!await rolemanager.RoleExistsAsync(role))
                {
                    await rolemanager.CreateAsync(new IdentityRole(role));
                }


            }
        }
    }
}
