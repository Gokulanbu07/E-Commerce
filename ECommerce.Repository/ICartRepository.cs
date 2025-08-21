using ECommerce.Repository;
using System.Collections.Generic;
// Removed using System.Threading.Tasks;

namespace ECommerce.Repository
{
    public interface ICartRepository
    {
        Cart GetCartItemById(int cartId);
        List<Cart> GetCartItemsByUserId(int userId); // Changed to List<Cart>
        void AddCartItem(Cart cartItem); // Changed to void
        void UpdateCartItem(Cart cartItem); // Changed to void
        void DeleteCartItem(int cartId); // Changed to void
        Cart GetCartItemByProductAndUser(int userId, int productId);
        void ClearCart(int userId); // Changed to void
    }
}