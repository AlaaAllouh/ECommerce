namespace ECommerce.Models
{
    public class AdminIndexVm
    {
        public AdminIndexVm()
        {
            Users = new List<UserRoleVm>();
        }
        public List<UserRoleVm> Users { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; }
    }
}
