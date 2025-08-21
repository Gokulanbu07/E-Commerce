using ECommerce.BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
// Removed using System.Threading.Tasks; // Still synchronous on the backend, but controller returns Json
// using System.Security.Claims; // Uncomment if you implement actual user authentication

namespace NextCart_User_UI.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // IMPORTANT: Replace this with actual user ID from authentication
        private int GetCurrentUserId()
        {
            // For now, return a fixed ID or get from session/cookie if you have a simple login.
            return 1; // Dummy User ID
        }

        // POST: /Cart/AddToCart - Called via AJAX
        [HttpPost]
        public IActionResult AddToCart([FromBody] CartData cartData) // <--- Accepting JSON data
        {
            if (cartData == null || cartData.ProductId <= 0 || cartData.Quantity <= 0)
            {
                return Json(new { success = false, message = "Invalid data provided." });
            }

            int userId = GetCurrentUserId();
            var success = _cartService.AddToCart(userId, cartData.ProductId, cartData.Quantity);

            if (success)
            {
                return Json(new { success = true, message = "Product added to cart successfully!" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to add product to cart. Please check stock or try again." });
            }
        }

        // POST: /Cart/RemoveFromCart - Called via AJAX
        [HttpPost]
        public IActionResult RemoveFromCart(int cartId) // <--- Accepting simple parameter, but returning JSON
        {
            var success = _cartService.RemoveFromCart(cartId);
            if (success)
            {
                return Json(new { success = true, message = "Item removed from cart." });
            }
            else
            {
                return Json(new { success = false, message = "Failed to remove item from cart." });
            }
        }

        // POST: /Cart/UpdateQuantity - Called via AJAX
        [HttpPost]
        public IActionResult UpdateQuantity(int cartId, int quantity) // <--- Accepting simple parameters, but returning JSON
        {
            if (quantity <= 0)
            {
                return Json(new { success = false, message = "Quantity must be a positive number." });
            }

            var success = _cartService.UpdateCartItemQuantity(cartId, quantity);
            if (success)
            {
                return Json(new { success = true, message = "Cart quantity updated." });
            }
            else
            {
                return Json(new { success = false, message = "Failed to update quantity. Insufficient stock or invalid quantity." });
            }
        }

        // GET: /Cart/Index - Displays the current shopping cart (no change needed here)
        public IActionResult Index()
        {
            int userId = GetCurrentUserId();
            var cartItems = _cartService.GetCartDetails(userId);

            decimal totalPrice = cartItems.Sum(item => item.Quantity * item.Product.Price);
            ViewBag.TotalPrice = totalPrice;

            return View(cartItems);
        }

        // POST: /Cart/Checkout - Dummy checkout process (no change needed here)
        [HttpPost]
        public IActionResult Checkout()
        {
            int userId = GetCurrentUserId();
            _cartService.ClearUserCart(userId);
            TempData["SuccessMessage"] = "Your order has been placed successfully!";
            return RedirectToAction("OrderConfirmation");
        }

        public IActionResult OrderConfirmation()
        {
            return View();
        }

        // DTO for AddToCart AJAX request
        public class CartData
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}