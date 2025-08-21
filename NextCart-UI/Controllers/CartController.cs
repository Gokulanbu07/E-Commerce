using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;
using ECommerce.Repository.Entity;
using ECommerce.Repository.Models;
using ECommerce.Repository;
using ECommerce.Repository.RepoInterface;
using ECommerce.BusinessLogic.Interface;
using ECommerce.BusinessLogic.Services; // Needed for ICartService
using System.Threading.Tasks; // Added for async actions

namespace NextCart_User_UI.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductRepository _productRepository;
        private readonly UserManager<User> _userManager;

        public CartController(ICartService cartService, IProductRepository productRepository, UserManager<User> userManager)
        {
            _cartService = cartService;
            _productRepository = productRepository;
            _userManager = userManager;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // Removed GetCurrentCartItemCount as it's now handled by the service and the AddToCartResult DTO

        // POST: /Cart/AddToCart - Called via AJAX
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartData cartData) // Changed to async Task<IActionResult>
        {
            if (cartData == null || cartData.ProductId <= 0 || cartData.Quantity <= 0)
            {
                return Json(new { success = false, message = "Invalid data provided." });
            }

            string userId = GetCurrentUserId();

            // The commented-out login check below is kept as per your original code.
            // if (string.IsNullOrEmpty(userId))
            // {
            //     // User is not logged in
            //     return Json(new { success = false, message = "User not logged in. Please log in to add items to cart." });
            // }

            // Call the async service method
            var result = await _cartService.AddToCartAsync(userId, cartData.ProductId, cartData.Quantity);

            if (result.Success)
            {
                // Directly use the values returned from the service
                return Json(new { success = true, message = result.Message, newCartCount = result.NewCartCount, currentProductStock = result.CurrentProductStock });
            }
            else
            {
                return Json(new { success = false, message = result.Message }); // Pass the message from the service
            }
        }

        // POST: /Cart/RemoveFromCart - Called via AJAX
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartId) // Changed to async Task<IActionResult>
        {
            string userId = GetCurrentUserId();

            var success = _cartService.RemoveFromCart(cartId);
            int newCartCount = await _cartService.GetCartItemCountAsync(userId); // Call async service method

            if (success)
            {
                return Json(new { success = true, message = "Item removed from cart.", newCartCount = newCartCount });
            }
            else
            {
                return Json(new { success = false, message = "Failed to remove item from cart." });
            }
        }

        // POST: /Cart/UpdateQuantity - Called via AJAX
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartId, int quantity) // Changed to async Task<IActionResult>
        {
            if (quantity <= 0)
            {
                return Json(new { success = false, message = "Quantity must be a positive number." });
            }

            string userId = GetCurrentUserId();

            var success = _cartService.UpdateCartItemQuantity(cartId, quantity);
            int newCartCount = await _cartService.GetCartItemCountAsync(userId); // Call async service method

            if (success)
            {
                return Json(new { success = true, message = "Cart quantity updated.", newCartCount = newCartCount });
            }
            else
            {
                return Json(new { success = false, message = "Failed to update quantity. Insufficient stock or invalid quantity." });
            }
        }

        // GET: /Cart/Index - Displays the current shopping cart
        public async Task<IActionResult> Index() // Changed to async Task<IActionResult>
        {
            string userId = GetCurrentUserId();

            var cartItems = _cartService.GetCartDetails(userId);

            // The commented-out login check below is kept as per your original code.
            // if (string.IsNullOrEmpty(userId))
            // {
            //     // User not logged in, redirect to login or show empty cart message
            //     return RedirectToPage("/Account/Login", new { area = "Identity" }); // Or return View("EmptyCart")
            // }

            // Ensure Product is eager-loaded in GetCartDetails for Price access
            decimal totalPrice = cartItems.Sum(item => item.Quantity * item.UnitPrice);
            ViewBag.TotalPrice = totalPrice;
            ViewBag.CartItemsCount = await _cartService.GetCartItemCountAsync(userId); // Call async service method
            return View(cartItems);
        }

        // POST: /Cart/ClearCart - Called via form submission
        [HttpPost]
        [ValidateAntiForgeryToken] // Important for security with POST requests
        public IActionResult ClearCart()
        {
            string userId = GetCurrentUserId();

            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "User not logged in. Cannot clear cart.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            _cartService.ClearUserCart(userId);
            return RedirectToAction("Index"); // Redirect back to the cart index page
        }

        // DTO for AddToCart AJAX request
        public class CartData
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}