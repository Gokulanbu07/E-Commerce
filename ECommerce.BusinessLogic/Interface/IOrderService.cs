using ECommerce.Repository.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Repository.Entity;
using ECommerce.Repository.RepoInterface;

namespace ECommerce.BusinessLogic.Interface
{
    public interface IOrderService
    {
        /// <summary>
        /// Creates a new order based on user's cart items and checkout information.
        /// </summary>
        /// <param name="userId">The ID of the user placing the order.</param>
        /// <param name="checkoutData">The shipping and contact details for the order.</param>
        /// <param name="cartItems">The list of items from the user's cart.</param>
        /// <returns>The newly created Order object.</returns>
        Task<Order> CreateOrderAsync(string userId, CheckoutViewModel checkoutData, List<Cart> cartItems);

        /// <summary>
        /// Updates the status of an existing order.
        /// </summary>
        /// <param name="orderId">The ID of the order to update.</param>
        /// <param name="newStatus">The new status for the order (e.g., "Processing", "Shipped", "Delivered").</param>
        /// <returns>True if the order status was updated successfully, false otherwise.</returns>
        Task<bool> UpdateOrderStatusAsync(string orderId, string newStatus);

        /// <summary>
        /// Retrieves the details of a specific order.
        /// </summary>
        /// <param name="orderId">The ID of the order to retrieve.</param>
        /// <returns>The Order object if found, otherwise null.</returns>
        Task<Order?> GetOrderDetailsAsync(string orderId);

        /// <summary>
        /// Retrieves all orders placed by a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose orders are to be retrieved.</param>
        /// <returns>An enumerable collection of Order objects for the specified user.</returns>
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
    }
}