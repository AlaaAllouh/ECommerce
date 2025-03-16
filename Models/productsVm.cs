using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace ECommerce.Models
{
    public class productsVm
    {
       

        [Display(Name ="اسم المنتج ")]
        [Required]
        public string productName { get; set; }
        [Display(Name ="اسم القسم")]
        [Required]
        public string productCat { get; set; }
        [Display(Name ="سعر المنتج")]
        [Required]
        public double productPrice{ get; set; }
        [Display(Name ="الكمية")]
        [Required]
        public int productQty { get; set; }



    }
}
