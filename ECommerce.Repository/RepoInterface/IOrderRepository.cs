using ECommerce.Repository.Entity;
using ECommerce.Repository.Models;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace ECommerce.Repository.RepoInterface

{

    public interface IOrderRepository

    {

        Task AddOrderAsync(Order order);

        Task UpdateOrderAsync(Order order);

        Task<Order> GetOrderByIdAsync(string orderId);

        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);

        Task<IEnumerable<Order>> GetAllOrdersAsync(); // For potential admin view

        // Methods for OrderItems (if direct access is needed from repository)

        Task AddOrderItemAsync(OrderItem orderItem);

        Task AddOrderItemsAsync(IEnumerable<OrderItem> orderItems);

    }

}

