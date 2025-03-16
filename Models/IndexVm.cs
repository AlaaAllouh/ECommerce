namespace ECommerce.Models
{
    public class IndexVm
    {
        //هاي الكونستركتور الهدف منه في حال ارسل اليوز مودل معين وكان فارغ مايعطيني ايرور
        public IndexVm()
        {
            Categories = new List<Category>();
            Products = new List<Product>();
            Reviews = new List<Review>();
            LatestProduct = new List<Product>();
        }
        public List<Category> Categories { get; set; }
        public List<Product> Products { get; set; }
        public List<Review> Reviews { get; set; }
        public List<Product> LatestProduct { get; set; }
    }
}
