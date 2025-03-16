using ECommerce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIManageProducts : ControllerBase
    {
        SouqContext db = new SouqContext();

        [HttpGet("getProducts")]
        public IActionResult getProducts()
        {
            return Ok(db.Products.Include(x => x.Cat).Select(x => new
            {
                x.Name,
                x.Id,
                x.Price,
                x.Qty,
                catname = x.Cat == null ? "" : x.Cat.Name
            }).ToList());
        }
        [HttpPost("saveProducts")]
        public IActionResult saveProducts(Product p)
        {
            try
            {
                var result = db.Products.Add(p);
                db.SaveChanges();

            }

            catch
            {

            }
            return Ok();
        }
    
    [HttpDelete("deleteProduct/{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            db.SaveChanges();

            return Ok(new { message = "تم الحذف بنجاح!" });
        }

    }
}