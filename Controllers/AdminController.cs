using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public UserManager<IdentityUser> _userManager;
        public RoleManager<IdentityRole> _roleManager;
        public ApplicationDbContext context;
        SouqContext db = new SouqContext();
        public AdminController(ILogger<HomeController> logger, UserManager<IdentityUser> user, RoleManager<IdentityRole> role, ApplicationDbContext identity)
        {
            _logger = logger;
            _userManager = user;
            _roleManager = role;
            context = identity;
        }

        public async Task<IActionResult> Index(string searchTerm, int pageNumber = 1)
        {
            const int PageSize = 10;

            // 1) استعلام المستخدمين مع فلترة البحث
            var query = _userManager.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(u => u.Email.Contains(searchTerm) || u.UserName.Contains(searchTerm));

            // 2) احسب العدد الكلي وعدد الصفحات
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            // 3) جلب صفحة المستخدمين
            var usersPage = await query
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // 4) تعبئة ViewModel
            var vm = new AdminIndexVm
            {
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                SearchTerm = searchTerm
            };

            foreach (var user in usersPage)
            {
                var roles = await _userManager.GetRolesAsync(user);
                vm.Users.Add(new UserRoleVm { User = user, userRoles = roles.ToList() });
            }

            ViewBag.AllRoles = await _roleManager.Roles.ToListAsync();
            return View(vm);
        }


        public async Task<IActionResult> addRoleToUser(string userid, string rolename)
        {
            var user = await _userManager.FindByIdAsync(userid);
            var result = await _userManager.AddToRoleAsync(user, rolename);
            if (!result.Succeeded)
            {
                await _userManager.RemoveFromRoleAsync(user, rolename);
            }
            return RedirectToAction("Index");
        }
    }




    //----------------------------------------------------------------------------------------------------------------------------------------------
    // var user = await _userManager.Users.ToListAsync();
    //  Onther way without search + pagination . 
    //        List<UserRoleVm> result = new List<UserRoleVm>();
    //        foreach (var item in user)
    //        {
    //            if (User != null)
    //            {
    //                var Roles = await _userManager.GetRolesAsync(item);
    //                result.Add(new UserRoleVm { User = item, userRoles = Roles.ToList() });
    //            }
    //        }
    //        ViewBag.AllRoles = await _roleManager.Roles.ToListAsync();
    //        return View(result);
    //    }


}
    