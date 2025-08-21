using ECommerce.Repository.Entity;
using ECommerce.Repository.RepoInterface;
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

        public List<Cart> GetCartItemsByUserId(string userId) // Changed userId type from int to string
        {
            return _context.Carts
                                 .Include(c => c.Product)
                                 .Where(c => c.UserId == userId) // No change needed here as types now match
                                 .ToList();
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

        public Cart GetCartItemByProductAndUser(string userId, int productId) // Changed userId type from int to string
        {
            return _context.Carts
            .Where(c => c.UserId == userId && c.ProductId == productId) // No change needed here as types now match
            .FirstOrDefault();
        }

        public void ClearCart(string userId) // Changed userId type from int to string
        {
            var cartItems = _context.Carts.Where(c => c.UserId == userId).ToList(); // No change needed here as types now match
            _context.Carts.RemoveRange(cartItems);
            _context.SaveChanges();
        }
    }
}