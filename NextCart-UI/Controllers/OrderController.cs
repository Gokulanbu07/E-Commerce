using ECommerce.Repository.Models;

using Microsoft.AspNetCore.Mvc;

using System.Linq;

using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity; // Needed for UserManager

using System.Threading.Tasks;
using ECommerce.Repository.Entity;
using ECommerce.BusinessLogic.Interface;

namespace NextCart_User_UI.Controllers

{

    [Authorize] // Ensure only logged-in users can access order actions

    public class OrderController : Controller

    {

        private readonly IOrderService _orderService;

        private readonly ICartService _cartService; // Still needed to get cart items for order creation

        private readonly UserManager<User> _userManager; // To get user details like email for order creation

        public OrderController(IOrderService orderService, ICartService cartService, UserManager<User> userManager)

        {

            _orderService = orderService;

            _cartService = cartService;

            _userManager = userManager;

        }

        private string GetCurrentUserId()

        {

            return User.FindFirstValue(ClaimTypes.NameIdentifier);

        }

        // GET: /Order/CheckoutPage - Displays the checkout form

        public IActionResult CheckoutPage()

        {

            string userId = GetCurrentUserId();

            if (string.IsNullOrEmpty(userId))

            {

                return RedirectToPage("/Account/Login", new { area = "Identity" });

            }

            // Note: Cart.UserId is 'int', while IdentityUser.Id is 'string'.

            // Parsing to int as per existing CartService/Repository expectations.

            var cartItems = _cartService.GetCartDetails(userId);

            if (cartItems == null || !cartItems.Any())

            {

                TempData["ErrorMessage"] = "Your cart is empty. Please add items before checking out.";

                return RedirectToAction("Index", "Cart"); // Redirect back to cart if empty

            }

            var checkoutViewModel = new CheckoutViewModel

            {

                // Pre-populate with default user address/phone if you have it in your User model

                // For now, it will be empty fields for user to fill.

            };

            ViewBag.CartItems = cartItems; // Pass cart items to the view for summary

            ViewBag.TotalPrice = cartItems.Sum(item => item.Quantity * item.UnitPrice);

            return View(checkoutViewModel);

        }

        // POST: /Order/ProcessCheckout - Action to handle submission of the checkout form

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ProcessCheckout(CheckoutViewModel model)

        {

            string userId = GetCurrentUserId();

            if (string.IsNullOrEmpty(userId))

            {

                return RedirectToPage("/Account/Login", new { area = "Identity" });

            }

            if (!ModelState.IsValid)

            {

                // Re-populate cart items for the view if validation fails

                // Note: Cart.UserId is 'int', while IdentityUser.Id is 'string'.

                // Parsing to int as per existing CartService/Repository expectations.

                var cartItems = _cartService.GetCartDetails(userId);

                ViewBag.CartItems = cartItems;

                ViewBag.TotalPrice = cartItems.Sum(item => item.Quantity * item.UnitPrice);

                return View("CheckoutPage", model); // Return to view with validation errors

            }

            // Note: Cart.UserId is 'int', while IdentityUser.Id is 'string'.

            // Parsing to int as per existing CartService/Repository expectations.

            var cartItemsToOrder = _cartService.GetCartDetails(userId);

            if (cartItemsToOrder == null || !cartItemsToOrder.Any())

            {

                TempData["ErrorMessage"] = "Your cart is empty. Please add items before checking out.";

                return RedirectToAction("Index", "Cart");

            }

            try

            {

                // Create the order using the OrderService

                var order = await _orderService.CreateOrderAsync(userId, model, cartItemsToOrder.ToList());

                if (order != null)

                {

                    // Clear the user's cart after successful order creation

                    // Note: Cart.UserId is 'int', while IdentityUser.Id is 'string'.

                    // Parsing to int as per existing CartService/Repository expectations.

                    _cartService.ClearUserCart(userId);

                    // Redirect to the Order Summary page for payment

                    return RedirectToAction("OrderSummary", new { orderId = order.OrderId });

                }

                else

                {

                    TempData["ErrorMessage"] = "Failed to create order. Please try again.";

                    return RedirectToAction("Index", "Cart");

                }

            }

            catch (System.Exception ex)

            {

                // Log the exception (e.g., using ILogger)

                TempData["ErrorMessage"] = $"An error occurred during checkout: {ex.Message}";

                // Re-populate cart items for the view if an exception occurs

                // Note: Cart.UserId is 'int', while IdentityUser.Id is 'string'.

                // Parsing to int as per existing CartService/Repository expectations.

                var cartItems = _cartService.GetCartDetails(userId);

                ViewBag.CartItems = cartItems;

                ViewBag.TotalPrice = cartItems.Sum(item => item.Quantity * item.UnitPrice);

                return View("CheckoutPage", model);

            }

        }

        // GET: /Order/OrderSummary - Displays order details and a payment button

        public async Task<IActionResult> OrderSummary(string orderId)

        {

            if (string.IsNullOrEmpty(orderId))

            {

                return RedirectToAction("OrderHistory"); // Redirect if no order ID provided

            }

            var order = await _orderService.GetOrderDetailsAsync(orderId);

            // Ensure the order exists and belongs to the current user

            if (order == null || order.UserId != GetCurrentUserId())

            {

                TempData["ErrorMessage"] = "Order not found or you do not have access.";

                return RedirectToAction("OrderHistory");

            }

            return View(order); // Pass the Order model to the view

        }

        // GET: /Order/OrderConfirmation - Displays final order confirmation details

        public async Task<IActionResult> OrderConfirmation(string orderId)

        {

            if (string.IsNullOrEmpty(orderId))

            {

                TempData["ErrorMessage"] = "No order ID provided for confirmation.";

                return RedirectToAction("OrderHistory");

            }

            var order = await _orderService.GetOrderDetailsAsync(orderId);

            if (order == null || order.UserId != GetCurrentUserId())

            {

                TempData["ErrorMessage"] = "Order not found or you do not have access.";

                return RedirectToAction("OrderHistory");

            }

            // Pass payment status from TempData if set by PaymentController.ProcessPayment

            ViewBag.PaymentStatus = TempData["PaymentStatus"] as string;

            return View(order);

        }

        // GET: /Order/OrderHistory - Displays all orders for the logged-in user

        public async Task<IActionResult> OrderHistory()

        {

            string userId = GetCurrentUserId();

            if (string.IsNullOrEmpty(userId))

            {

                return RedirectToPage("/Account/Login", new { area = "Identity" });

            }

            var userOrders = await _orderService.GetUserOrdersAsync(userId);

            return View(userOrders);

        }

        // GET: /Order/OrderDetails - Displays detailed view of a single order

        public async Task<IActionResult> OrderDetails(string orderId)

        {

            if (string.IsNullOrEmpty(orderId))

            {

                TempData["ErrorMessage"] = "Invalid order ID.";

                return RedirectToAction("OrderHistory");

            }

            var order = await _orderService.GetOrderDetailsAsync(orderId);

            if (order == null || order.UserId != GetCurrentUserId())

            {

                TempData["ErrorMessage"] = "Order not found or you do not have access.";

                return RedirectToAction("OrderHistory");

            }

            return View(order);

        }

    }

}

