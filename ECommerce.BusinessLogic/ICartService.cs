using ECommerce.Repository;
// To use the Cart entity model
using System.Collections.Generic;
// Removed using System.Threading.Tasks;

namespace ECommerce.BusinessLogic
{
    public interface ICartService
    {
        bool AddToCart(int userId, int productId, int quantity); // Changed to bool
        bool RemoveFromCart(int cartId); // Changed to bool
        bool UpdateCartItemQuantity(int cartId, int quantity); // Changed to bool
        List<Cart> GetCartDetails(int userId); // Changed to List<Cart>
        void ClearUserCart(int userId); // Changed to void
    }
}