using EleVehicleDealer.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EleVehicleDealer.Presentation.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            // Ki?m tra ??ng nh?p
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                TempData["Error"] = "Please login to view your cart.";
                return RedirectToAction("Login", "Account");
            }

            var cart = await _cartService.GetCartAsync();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int vehicleId, int quantity = 1)
        {
            // Ki?m tra ??ng nh?p
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                TempData["Error"] = "Please login to add items to cart.";
                return RedirectToAction("Login", "Account");
            }

            // L?y image mapping t? session
            var imageMapping = HttpContext.Session.GetString("VehicleImageMapping");
            Dictionary<int, string> mapping = new Dictionary<int, string>();
            
            if (!string.IsNullOrEmpty(imageMapping))
            {
                mapping = System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, string>>(imageMapping) ?? new Dictionary<int, string>();
            }

            // L?y tên ?nh cho xe này
            string imageName = mapping.ContainsKey(vehicleId) ? mapping[vehicleId] : "default.jpg";

            // Add to cart v?i imageName
            await _cartService.AddToCartAsync(vehicleId, quantity, imageName);
            TempData["Success"] = "Vehicle added to cart successfully!";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int vehicleId)
        {
            // Ki?m tra ??ng nh?p
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                TempData["Error"] = "Please login to manage your cart.";
                return RedirectToAction("Login", "Account");
            }

            await _cartService.RemoveFromCartAsync(vehicleId);
            TempData["Success"] = "Vehicle removed from cart!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int vehicleId, int quantity)
        {
            // Ki?m tra ??ng nh?p
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                TempData["Error"] = "Please login to manage your cart.";
                return RedirectToAction("Login", "Account");
            }

            await _cartService.UpdateQuantityAsync(vehicleId, quantity);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            // Ki?m tra ??ng nh?p
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                TempData["Error"] = "Please login to manage your cart.";
                return RedirectToAction("Login", "Account");
            }

            await _cartService.ClearCartAsync();
            TempData["Success"] = "Cart cleared!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GetCartCount()
        {
            // Ki?m tra ??ng nh?p
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return Json(new { count = 0 });
            }

            var cart = await _cartService.GetCartAsync();
            return Json(new { count = cart.TotalItems });
        }
    }
}
