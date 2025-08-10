using System.Diagnostics;
using System.Linq;
using System.Net.Quic;
using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace ECommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public UserManager<IdentityUser> _userManager;
        public RoleManager<IdentityRole> _roleManager;
        public ApplicationDbContext context;
        SouqContext db = new SouqContext();
        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser>user, RoleManager<IdentityRole>role, ApplicationDbContext identity)
        {
            _logger = logger;
            _userManager = user; 
            _roleManager = role;
            context = identity;
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
       // [Authorize(Roles ="Admin")]
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
        public IActionResult addOrder(OrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "تأكد من تعبئة كل الحقول.";
                return RedirectToAction("Cart");
            }

            // تحقق وهمي من البطاقة
            if (model.CardNumber.Length != 16 || model.CVV.Length != 3)
            {
                TempData["Error"] = "معلومات البطاقة غير صحيحة.";
                return RedirectToAction("Cart");
            }

            // استرجاع السلة الخاصة بالمستخدم
            var userId = User.Identity?.Name;
            var cartItems = db.Carts.Where(c => c.UserId == userId).Include(c => c.Product).ToList();

            if (!cartItems.Any())
            {
                TempData["Error"] = "السلة فارغة.";
                return RedirectToAction("Cart");
            }

            // إنشاء الطلب
            var order = new Order
            {
                Name = model.Name,
                Email = model.Email,
                Aderss = model.Aderss,
                Mobile = model.Mobile,
                UserId = userId,
                Total = cartItems.Sum(c => c.Product.Price * c.Qty),
                OrderDetails = new List<OrderDetail>()
            };

            foreach (var item in cartItems)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    Price = item.Product.Price,
                    Qty = item.Qty,
                    Total = item.Product.Price * item.Qty
                });
            }

            db.Orders.Add(order);
            db.SaveChanges();

            // حذف السلة
            db.Carts.RemoveRange(cartItems);
            db.SaveChanges();

            TempData["Success"] = "✅ تم الدفع وحفظ الطلب بنجاح!";
            return RedirectToAction("OrderSuccess");
        }
        public IActionResult OrderSuccess()
        {
            return View();
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
        public IActionResult ProductDetails(int id)
        {
            // جلب بيانات المنتج مع التقييمات والصور والفئة
            var product = db.Products
                .Include(x => x.Cat)
                .Include(x => x.ProductImages)
                .Include(x => x.Reviews)       // جلب التقييمات مع المنتج
                .FirstOrDefault(x => x.Id == id);

            if (product == null)
                return NotFound();

            // المنتجات المقترحة بناءً على الطلبات السابقة أو عشوائية
            var recommendedProducts = db.OrderDetails
                .Where(od => od.ProductId == id)
                .SelectMany(od => db.OrderDetails
                    .Where(r => r.OrderId == od.OrderId && r.ProductId != id))
                .Select(rp => rp.Product)
                .Distinct()
                .Take(4)
                .ToList();

            if (!recommendedProducts.Any())
            {
                recommendedProducts = db.Products
                    .Where(p => p.Id != id)
                    .OrderBy(r => Guid.NewGuid())
                    .Take(4)
                    .ToList();
            }

            ViewBag.RecommendedProducts = recommendedProducts;

            // **أضف هذا السطر**
            ViewBag.ProductId = id;

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
      //  [Authorize]
        public IActionResult myReview(Review model)
        {
            Console.WriteLine($"ProductId: {model.ProductId}, Name: {model.Name}, Email: {model.Email}");

            
                model.Name = model.Name;
                model.Email = model.Email;
                model.Subject = model.Subject?.Trim();
                model.Description = model.Description?.Trim();

                db.Reviews.Add(model);
                db.SaveChanges();

                // إعادة التوجيه لصفحة المنتج مع ظهور التقييم الجديد
                return RedirectToAction("ProductDetails", new { id = model.ProductId });
            

            
            return RedirectToAction("ProductDetails", new { id = model.ProductId });
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

        [HttpPost]
        [Authorize]
        public IActionResult AddToFavorite(int productId)
        {
            var userId = User.Identity?.Name ?? "";

            var existing = db.Wishlists.FirstOrDefault(f => f.ProductId == productId && f.UserId == userId);
            if (existing == null)
            {
                _ = db.Wishlists.Add(new Wishlist
                {
                    UserId = userId,
                    ProductId = productId
                });
                db.SaveChanges();
            }

            return RedirectToAction("ProductDetails", new { id = productId });
        }

        [Authorize]
        public IActionResult MyFavorites()
        {
            var userId = User.Identity?.Name ?? "";
            var favorites = db.Wishlists
                              .Include(f => f.Product)
                              .Where(f => f.UserId == userId)
                              .ToList();

            return View(favorites);
        }
        [Authorize]
        public IActionResult RemoveFromFavorite(int productId)
        {
            var userId = User.Identity?.Name ?? "";
            var favorite = db.Wishlists.FirstOrDefault(f => f.ProductId == productId && f.UserId == userId);
            if (favorite != null)
            {
                db.Wishlists.Remove(favorite);
                db.SaveChanges();
            }

            return RedirectToAction("MyFavorites");
        }



    }
}
