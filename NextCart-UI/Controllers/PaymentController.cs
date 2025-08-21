using Microsoft.AspNetCore.Mvc;
using ECommerce.Repository.Models; // For PaymentInputViewModel, PaymentResultViewModel
using System.Globalization; // Needed for DateTime.TryParseExact
using Microsoft.AspNetCore.Authorization; // For [Authorize] attribute
using System.Threading.Tasks;
using ECommerce.Repository.Entity;
using ECommerce.BusinessLogic.Interface; // For async methods

namespace NextCart_User_UI.Controllers
{
    [Authorize] // Ensure only logged-in users can access payment actions
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService; // Inject IOrderService

        public PaymentController(IPaymentService paymentService, IOrderService orderService) // Constructor injection
        {
            _paymentService = paymentService;
            _orderService = orderService; // Assign injected service
        }

        // GET: /Payment/ProcessPayment
        // This action handles the "proceed payment" button click from an order summary page.
        // It now accepts 'amount' from the query string for initial display.
        [HttpGet]
        public async Task<IActionResult> ProcessPayment(string? orderId, decimal? amount) // <--- Modified: Added decimal? amount
        {
            var model = new PaymentInputViewModel();

            if (string.IsNullOrEmpty(orderId))
            {
                TempData["ErrorMessage"] = "Order ID is required to process payment.";
                return RedirectToAction("OrderHistory", "Order"); // Redirect to order history
            }

            // Fetch the order from the database. This is still necessary to validate the orderId
            // and have a fallback for the amount, and for the security check in the POST method.
            var order = await _orderService.GetOrderDetailsAsync(orderId);

            if (order == null)
            {
                TempData["ErrorMessage"] = $"Order with ID '{orderId}' not found.";
                return RedirectToAction("OrderHistory", "Order");
            }
            else
            {
                model.OrderId = order.OrderId;
                // For initial display on the page, use the 'amount' passed in the query string if available.
                // If 'amount' is null (e.g., direct navigation or link didn't include it),
                // fall back to the order's TotalAmount from the database.
                model.Amount = amount ?? order.TotalAmount; // <--- MODIFIED: Prioritize passed amount for display
            }

            model.PaymentMethod = PaymentMethod.Online; // Default payment method to Online
            return View(model);
        }


        // POST: /Payment/ProcessPayment - Handles both COD and Online Payment submissions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(PaymentInputViewModel model)
        {
            // SECURITY CRITICAL: RE-FETCH ORDER FROM DATABASE FOR AMOUNT VALIDATION.
            // Do NOT trust model.Amount directly from the client for the actual transaction.
            var order = await _orderService.GetOrderDetailsAsync(model.OrderId!);

            if (order == null)
            {
                ModelState.AddModelError("OrderId", "Order not found. Cannot process payment.");
            }
            // IMPORTANT: Always compare the client-submitted amount (model.Amount) with the
            // server-side fetched authoritative amount (order.TotalAmount).
            else if (order.TotalAmount != model.Amount) // This check is crucial for security!
            {
                ModelState.AddModelError("Amount", "The order amount does not match. Please try again.");
                // Potentially log this as a suspicious activity for security monitoring.
            }

            // --- Manual Validation for Online Payment Details if selected ---
            if (model.PaymentMethod == PaymentMethod.Online) // Check for online payment specific fields
            {
                if (string.IsNullOrEmpty(model.CardNumber) || model.CardNumber.Length < 15 || model.CardNumber.Length > 19)
                {
                    ModelState.AddModelError("CardNumber", "Card Number is required and must be between 15-19 digits.");
                }

                if (string.IsNullOrEmpty(model.ExpiryDateString))
                {
                    ModelState.AddModelError("ExpiryDateString", "Expiry Date is required.");
                }
                else
                {
                    DateTime expiryDate;
                    bool parsed = false;
                    // Try parsing MM/YY
                    if (DateTime.TryParseExact(model.ExpiryDateString, "MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out expiryDate))
                    {
                        parsed = true;
                    }
                    // Try parsing MM/YYYY
                    else if (DateTime.TryParseExact(model.ExpiryDateString, "MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out expiryDate))
                    {
                        parsed = true;
                    }

                    if (!parsed)
                    {
                        ModelState.AddModelError("ExpiryDateString", "Invalid Expiry Date format. Use MM/YY or MM/YYYY.");
                    }
                    else
                    {
                        // Set expiry date to the last day of the month for comparison
                        expiryDate = new DateTime(expiryDate.Year, expiryDate.Month, DateTime.DaysInMonth(expiryDate.Year, expiryDate.Month));
                        if (expiryDate < DateTime.Now.Date) // Compare with current date, not just month/year
                        {
                            ModelState.AddModelError("ExpiryDateString", "Expiry Date cannot be in the past.");
                        }
                    }
                }

                if (string.IsNullOrEmpty(model.CVV) || (model.CVV.Length != 3 && model.CVV.Length != 4))
                {
                    ModelState.AddModelError("CVV", "CVV is required (3 or 4 digits).");
                }
            }


            if (!ModelState.IsValid)
            {
                // If validation fails, return to the view with the current model state
                return View(model);
            }

            Payment paymentResult;
            string? redirectUrl = null;

            if (model.PaymentMethod == PaymentMethod.COD)
            {
                // Use the authoritative order.TotalAmount from the database for the payment record
                paymentResult = await _paymentService.ProcessCodPaymentAsync(order.OrderId, order.TotalAmount);
                return RedirectToAction("PaymentStatus", new { paymentId = paymentResult.PaymentId });
            }
            else // PaymentMethod.Online
            {
                // Use the authoritative order.TotalAmount from the database for the payment record
                var (payment, simulatedRedirectUrl) = await _paymentService.ProcessOnlinePaymentAsync(order.OrderId, order.TotalAmount);
                paymentResult = payment;
                redirectUrl = simulatedRedirectUrl;

                if (paymentResult.PaymentStatus == ECommerce.Repository.Entity.PaymentStatus.FAILED || string.IsNullOrEmpty(redirectUrl))
                {
                    // If online payment setup fails (e.g., gateway communication error)
                    var resultModel = new PaymentResultViewModel
                    {
                        PaymentId = paymentResult.PaymentId,
                        OrderId = paymentResult.OrderId,
                        Amount = paymentResult.Amount,
                        PaymentStatus = ECommerce.Repository.Entity.PaymentStatus.FAILED,
                        PaymentMethod = model.PaymentMethod,
                        PaymentDate = paymentResult.PaymentDate,
                        IsSuccess = false,
                        Message = "Online payment setup failed. Please try again."
                    };
                    return View("PaymentStatus", resultModel);
                }
                return Redirect(redirectUrl); // Redirect to simulated payment gateway
            }
        }

        // GET: /Payment/PayPalReturn - Handles callback from simulated PayPal
        // In a real scenario, this would be a POST from the gateway with signed data.
        [HttpGet]
        public async Task<IActionResult> PayPalReturn(string paymentId, string token)
        {
            var payment = await _paymentService.HandlePayPalReturnAsync(paymentId, token);
            return RedirectToAction("PaymentStatus", new { paymentId = payment.PaymentId });
        }

        // GET: /Payment/PaymentStatus - Displays the final status of a payment
        public async Task<IActionResult> PaymentStatus(string paymentId, string? message)
        {
            var payment = await _paymentService.GetPaymentDetailsAsync(paymentId);
            PaymentResultViewModel resultModel;

            if (payment.Amount == null)
            {
                resultModel = new PaymentResultViewModel
                {
                    PaymentId = paymentId,
                    OrderId = "N/A", // Or a default value if order is not found
                    Amount = 0m,
                    PaymentStatus = ECommerce.Repository.Entity.PaymentStatus.FAILED,
                    PaymentMethod = PaymentMethod.Online, // Default to online if payment record is missing
                    PaymentDate = DateTime.UtcNow,
                    IsSuccess = false,
                    Message = message ?? "The requested payment could not be found."
                };
            }
            else
            {
                resultModel = new PaymentResultViewModel
                {
                    PaymentId = payment.PaymentId,
                    OrderId = payment.OrderId,
                    Amount = payment.Amount,
                    PaymentStatus = payment.PaymentStatus,
                    PaymentMethod = payment.PaymentMethod,
                    PaymentDate = payment.PaymentDate,
                    IsSuccess = (payment.PaymentStatus == ECommerce.Repository.Entity.PaymentStatus.COMPLETED || payment.PaymentStatus == ECommerce.Repository.Entity.PaymentStatus.COD),
                    Message = message ?? (payment.PaymentStatus == ECommerce.Repository.Entity.PaymentStatus.COMPLETED ? "Payment completed successfully!" :
                                        (payment.PaymentStatus == ECommerce.Repository.Entity.PaymentStatus.COD ? "Order placed successfully (Cash on Delivery)!" :
                                        $"Payment {payment.PaymentStatus.ToString().ToLower()}."))
                };
            }
            return View(resultModel);
        }
    }
}
