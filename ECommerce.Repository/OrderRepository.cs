using ECommerce.Repository.Entity;
using ECommerce.Repository.Models;
using ECommerce.Repository.RepoInterface;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;

using System.Linq;

using System.Threading.Tasks;

namespace ECommerce.Repository

{

    public class OrderRepository : IOrderRepository

    {

        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)

        {

            _context = context;

        }

        public async Task AddOrderAsync(Order order)

        {

            await _context.Orders.AddAsync(order);

            await _context.SaveChangesAsync();

        }

        public async Task UpdateOrderAsync(Order order)

        {

            _context.Orders.Update(order);

            await _context.SaveChangesAsync();

        }

        public async Task<Order> GetOrderByIdAsync(string orderId)

        {

            return await _context.Orders

                                 .Include(o => o.OrderItems) // Eager load order items

                                 .Include(o => o.Payments)   // Eager load payments

                                 .FirstOrDefaultAsync(o => o.OrderId == orderId);

        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)

        {

            return await _context.Orders

                                 .Where(o => o.UserId == userId)

                                 .Include(o => o.OrderItems) // Eager load order items for display

                                 .OrderByDescending(o => o.OrderDate)

                                 .ToListAsync();

        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()

        {

            return await _context.Orders

                                 .Include(o => o.OrderItems)

                                 .Include(o => o.User) // Include user details if needed for admin view

                                 .OrderByDescending(o => o.OrderDate)

                                 .ToListAsync();

        }

        public async Task AddOrderItemAsync(OrderItem orderItem)

        {

            await _context.OrderItems.AddAsync(orderItem);

            await _context.SaveChangesAsync(); // Note: Often order items are saved with the order in a single transaction

        }

        public async Task AddOrderItemsAsync(IEnumerable<OrderItem> orderItems)

        {

            await _context.OrderItems.AddRangeAsync(orderItems);

            // SaveChanges is typically called once for the whole order transaction,

            // but this method is here if needed for specific scenarios.

            await _context.SaveChangesAsync();

        }

    }

}