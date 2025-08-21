using ECommerce.Repository.Entity;

namespace ECommerce.Repository.RepoInterface
{
    public interface ICartRepository
    {
        Cart GetCartItemById(int cartId);
        List<Cart> GetCartItemsByUserId(string userId); // Changed from int to string
        void AddCartItem(Cart cartItem);
        void UpdateCartItem(Cart cartItem);
        void DeleteCartItem(int cartId);
        Cart GetCartItemByProductAndUser(string userId, int productId); // Changed from int to string
        void ClearCart(string userId); // Changed from int to string
    }
}