using System.Diagnostics;
using System.Linq;
using System.Net.Quic;
using ECommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace ECommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        SouqContext db = new SouqContext();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            IndexVm result = new IndexVm(); 
           result.Categories = db.Categories.ToList();
           result.Products = db.Products.ToList();
            result.Reviews = db.Reviews.ToList();
            result.LatestProduct = db.Products.OrderByDescending(x => x.EntryDate).Take(6).ToList();
            return View(result);
        }

        public IActionResult Categories()
        {
            var cats = db.Categories.ToList();
            //ViewBag.isAdmin = true;
            return View(cats);
        }
        [Authorize]
        public IActionResult Cart()
        {

            var cart = db.Carts.Include(x => x.Product).Where(x => x.UserId == User.Identity.Name).ToList();
            var total = cart.Sum(x => x.Price * x.Qty);
            ViewBag.Total = total;
            return View(cart);
        }
        [Authorize]
        public IActionResult AddToCart(int id)
        {
            var price = db.Products.Find(id).Price;
            var item=db.Carts.FirstOrDefault(x => x.ProductId == id && x.UserId==User.Identity.Name);
            if (item != null)
            {
                item.Qty += 1;
                
                db.SaveChanges();

            }
            else
            {
               
                db.Carts.Add(new Cart
                {
                    ProductId = id,
                    UserId = User.Identity.Name,
                    Qty = 1,
                    Price =price,
                    
                });
                db.SaveChanges();

            }
          return  RedirectToAction("Cart");
        }
        [HttpPost]
        [Authorize]
        public IActionResult addOrder(Order model)
        {
           
            var order = new Order
            {
                Name=model.Name,
                Email = model.Email,
                Aderss = model.Aderss,
                Mobile = model.Mobile,
                UserId = User.Identity.Name,
            };
            db.Orders.Add(order);
            var cartItem = db.Carts.Where(x => x.UserId == User.Identity.Name).ToList();
            var total = cartItem.Sum(x => x.Price * x.Qty);
         
            foreach (var item in cartItem)
            {
                
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    Qty = item.Qty,
                    Total= total
                });
            }
            db.Carts.RemoveRange(cartItem);

            db.SaveChanges();

            return RedirectToAction("Cart");
        }
        [Authorize]
        public IActionResult orders()
        {
            
            var orders = db.Orders.Include(x=>x.OrderDetails).ThenInclude(x=>x.Product).Where(x => x.UserId == User.Identity.Name).ToList();
           
            return View(orders);
        }

        public IActionResult Products(int id)
        {
            var Products = db.Products.Where(x=>x.Catid == id).ToList();
            var category = db.Categories.FirstOrDefault(y => y.Id==id);
            ViewBag.Category = category;
            return View(Products);
        }
        public IActionResult ProductDetails(int Id)
        {
            // جلب بيانات المنتج الحالي
            var product = db.Products
                .Include(x => x.Cat)
                .Include(x => x.ProductImages)
                .FirstOrDefault(x => x.Id == Id);

            // جلب المنتجات الموصى بها بناءً على الطلبات السابقة
            var recommendedProducts = db.OrderDetails
                .Where(od => od.ProductId == Id)
                .SelectMany(od => db.OrderDetails
                    .Where(r => r.OrderId == od.OrderId && r.ProductId != Id))
                .Select(rp => rp.Product)
                .Distinct()
                .Take(4)
                .ToList();

            // في حال عدم وجود منتجات ذات صلة، عرض منتجات عشوائية
            if (!recommendedProducts.Any())
            {
                recommendedProducts = db.Products
                    .Where(p => p.Id != Id) // استبعاد المنتج الحالي
                    .OrderBy(r => Guid.NewGuid()) // ترتيب عشوائي
                    .Take(4)
                    .ToList();
            }

            ViewBag.RecommendedProducts = recommendedProducts;

            return View(product);
        }


        public IActionResult RemoveProductFromCart(int Id)
        {
            var products = db.Carts.FirstOrDefault(x=>x.ProductId==Id);
            db.Carts.Remove(products);
            db.SaveChanges();
            return RedirectToAction("Cart");
        }


        [HttpGet]
        public IActionResult ProductSearch(string xname)
        {
            List<Product> products; // تجنب إعادة تعريف المتغير داخل الشروط

            if (string.IsNullOrEmpty(xname))
            {
                products = db.Products.ToList();
            }
            else
            {
                products = db.Products
                 .Where(x => x.Name.Contains(xname))
                             .ToList();
            }

            if (products == null || !products.Any())
            {
                ViewBag.Message = "لا توجد نتائج مطابقة للبحث.";
            }

            return View(products);
        }


        [HttpPost]
        public IActionResult myReview(Review model) {
           
            db.Reviews.Add(new Review
            {
                Name = model.Name,
                Email = model.Email,
                Subject = model.Subject,
                Description = model.Description,
               
              
            });
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
       
        public IActionResult chart()
        {
            return View(); 
        }
        //https://localhost:7094/Home/GetAllProduct
            [HttpGet]
        public IActionResult GetAllProduct()
        {
            SouqContext db = new SouqContext();
            
            return Ok(db.Products.Select(x => new { x.Name, x.Qty, x.Price }).ToList());
        }
    }
}
