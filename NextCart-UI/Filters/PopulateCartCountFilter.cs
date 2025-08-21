using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // For ClaimTypes
using System.Threading.Tasks;
using ECommerce.BusinessLogic.Interface; // Namespace for your ICartService

namespace NextCart_UI.Filters
{
    public class PopulateCartCountFilter : IAsyncActionFilter
    {
        private readonly ICartService _cartService;

        // The filter itself needs to be injected with services
        public PopulateCartCountFilter(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 1. Execute the action first:
            // This is crucial. We let the controller action run, generate its ViewResult, etc.
            // This allows the controller to potentially set other ViewData/ViewBag properties
            // without being overwritten by our filter.
            var resultContext = await next(); // 'resultContext' contains the result of the action (e.g., ViewResult)

            // 2. Check if the result is a ViewResult and if the user is an authenticated CUSTOMER:
            if (resultContext.Result is ViewResult viewResult) // Only apply to views
            {
                var user = context.HttpContext.User;
                if (user.Identity.IsAuthenticated && user.IsInRole("CUSTOMER"))
                {
                    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!string.IsNullOrEmpty(userId))
                    {
                        // Get the cart count using your service
                        int cartItemCount = await _cartService.GetCartItemCountAsync(userId);
                        // Inject it into ViewData (which ViewBag uses)
                        viewResult.ViewData["CartItemCount"] = cartItemCount;
                        Console.WriteLine($"Cart count for user {userId}: {cartItemCount}"); // For debugging
                    }
                    else
                    {
                        // If user is authenticated but no userId claim, assume 0
                        viewResult.ViewData["CartItemCount"] = 0;
                        Console.WriteLine("Authenticated user but no User ID claim found. Cart count set to 0.");
                    }
                }
                else
                {
                    // If not authenticated or not a customer, cart count is 0
                    viewResult.ViewData["CartItemCount"] = 0;
                    Console.WriteLine("User not authenticated or not a CUSTOMER. Cart count set to 0.");
                }
            }
        }
    }
}