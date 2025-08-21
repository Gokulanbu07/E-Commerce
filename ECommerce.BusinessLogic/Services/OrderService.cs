using ECommerce.BusinessLogic.Interface;
using ECommerce.Repository.Entity;
using ECommerce.Repository.Models;
using ECommerce.Repository.RepoInterface;
using Microsoft.AspNetCore.Identity; // For UserManager to get user email
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.BusinessLogic.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<User> _userManager;
        //private readonly IProductRepository _productRepository; // Added to check product details if needed
        // Removed IPaymentRepository and IProductRepository from here.

        public OrderService(IOrderRepository orderRepository,
                            UserManager<User> userManager)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
            //_productRepository = productRepository;
        }

        public async Task<Order> CreateOrderAsync(string userId, CheckoutViewModel checkoutData, List<Cart> cartItems)
        {
            if (cartItems == null || !cartItems.Any())
            {
                throw new ArgumentException("Cannot create an order from an empty cart.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            //// --- START: New logic to update User's PhoneNumber ---
            //// If the phone number provided in the checkout data is different from the user's current stored phone number, update it.
            //// This ensures the user's profile always reflects the latest contact info used for an order.
            //if (user.PhoneNumber != checkoutData.PhoneNumber)
            //{
            //    user.PhoneNumber = checkoutData.PhoneNumber;
            //    var updateResult = await _userManager.UpdateAsync(user);

            //    if (!updateResult.Succeeded)
            //    {
            //        // Log the errors if updating the user's phone number fails.
            //        // This error might not be critical enough to stop the order creation,
            //        // but it's important to know if user profile updates are failing.
            //        Console.WriteLine($"Warning: Failed to update phone number for user {userId}. Errors:");
            //        foreach (var error in updateResult.Errors)
            //        {
            //            Console.WriteLine($"- {error.Description}");
            //        }
            //        // Consider implementing more robust error handling or notification here.
            //    }
            //}
            //// --- END: New logic to update User's PhoneNumber ---

            // Calculate total amount
            decimal totalAmount = cartItems.Sum(item => item.Quantity * item.UnitPrice);

            // Create Order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                OrderStatus = "Pending", // Initial status, payment will update this
                ShippingAddress = checkoutData.Address,
                ShippingCity = checkoutData.City,
                ShippingState = checkoutData.State,
                ShippingPostalCode = checkoutData.PostalCode,
                ShippingPhoneNumber = checkoutData.PhoneNumber,
                CustomerEmail = user.Email // Automatically fetched email
            };

            // Add OrderItems
            order.OrderItems = new List<OrderItem>();
            foreach (var cartItem in cartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    OrderItemId = Guid.NewGuid().ToString(),
                    ProductId = cartItem.ProductId,
                    ProductName = cartItem.ProductName,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.UnitPrice,
                    TotalPrice = cartItem.Quantity * cartItem.UnitPrice
                });
            }

            await _orderRepository.AddOrderAsync(order);

            return order;
        }

        // This method is now handled by PaymentService, and payment updates order status.
        // public async Task<bool> ProcessDummyPaymentAsync(string orderId, string paymentMethod) { ... }

        public async Task<bool> UpdateOrderStatusAsync(string orderId, string newStatus)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return false;
            }

            order.OrderStatus = newStatus;
            await _orderRepository.UpdateOrderAsync(order);
            return true;
        }

        public async Task<Order?> GetOrderDetailsAsync(string orderId)
        {
            return await _orderRepository.GetOrderByIdAsync(orderId);
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId)
        {
            return await _orderRepository.GetOrdersByUserIdAsync(userId);
        }
    }
}