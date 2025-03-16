using ECommerce.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    public class manageController : Controller
    {
        SouqContext db = new SouqContext();
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult getProducts()
        {
            return Ok(db.Products.ToList());


        }
    }
}