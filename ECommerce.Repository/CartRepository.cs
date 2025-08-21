using ECommerce.Repository; 
using ECommerce.Repository.Interfaces;
using Microsoft.EntityFrameworkCore; // Still needed for Include, Where etc.
using System.Collections.Generic;
using System.Linq;
// Removed using System.Threading.Tasks;

namespace ECommerce.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Cart GetCartItemById(int cartId)
        {
            return _context.Carts.Find(cartId); // Changed FindAsync to Find
        }

        public List<Cart> GetCartItemsByUserId(int userId)
        {
            return _context.Carts
                                 .Include(c => c.Product)
                                 .Where(c => c.UserId == userId)
                                 .ToList(); // Changed ToListAsync to ToList
        }

        public void AddCartItem(Cart cartItem)
        {
            _context.Carts.Add(cartItem); // Changed AddAsync to Add
            _context.SaveChanges(); // Changed SaveChangesAsync to SaveChanges
        }

        public void UpdateCartItem(Cart cartItem)
        {
            _context.Carts.Update(cartItem);
            _context.SaveChanges(); // Changed SaveChangesAsync to SaveChanges
        }

        public void DeleteCartItem(int cartId)
        {
            var cartItem = _context.Carts.Find(cartId); // Changed FindAsync to Find
            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                _context.SaveChanges(); // Changed SaveChangesAsync to SaveChanges
            }
        }

        public Cart GetCartItemByProductAndUser(int userId, int productId)
        {
            return _context.Carts
                                 .Where(c => c.UserId == userId && c.ProductId == productId)
                                 .FirstOrDefault(); // Changed FirstOrDefaultAsync to FirstOrDefault
        }

        public void ClearCart(int userId)
        {
            var cartItems = _context.Carts.Where(c => c.UserId == userId).ToList(); // Changed ToListAsync to ToList
            _context.Carts.RemoveRange(cartItems);
            _context.SaveChanges(); // Changed SaveChangesAsync to SaveChanges
        }
    }
}