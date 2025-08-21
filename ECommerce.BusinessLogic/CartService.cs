using ECommerce.BusinessLogic;
using ECommerce.Repository;
using ECommerce.Repository;
using ECommerce.Repository.Interfaces; // To use ICartRepository and IProductRepository
using System.Collections.Generic;
// Removed using System.Threading.Tasks;

namespace ECommerce.BusinessLogic
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository; // To check product stock and details

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public bool AddToCart(int userId, int productId, int quantity)
        {
            if (quantity <= 0)
            {
                return false; // Quantity must be positive
            }

            // Get product details to check stock
            var product = _productRepository.GetById(productId); // <<<--- CORRECTED METHOD NAME HERE
            if (product == null || product.StockQuantity < quantity)
            {
                // Product not found or not enough stock
                return false;
            }

            // Check if this product is already in the user's cart
            var existingCartItem = _cartRepository.GetCartItemByProductAndUser(userId, productId);

            if (existingCartItem != null)
            {
                // Update existing item quantity
                int newTotalQuantity = existingCartItem.Quantity + quantity;
                if (newTotalQuantity > product.StockQuantity)
                {
                    newTotalQuantity = product.StockQuantity; // Cap at available stock
                }
                existingCartItem.Quantity = newTotalQuantity;
                _cartRepository.UpdateCartItem(existingCartItem);
            }
            else
            {
                // Add new item to cart
                var newCartItem = new Cart
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                };
                _cartRepository.AddCartItem(newCartItem);
            }
            return true;
        }

        public bool RemoveFromCart(int cartId)
        {
            var cartItem = _cartRepository.GetCartItemById(cartId);
            if (cartItem == null)
            {
                return false; // Cart item not found
            }
            _cartRepository.DeleteCartItem(cartId);
            return true;
        }

        public bool UpdateCartItemQuantity(int cartId, int quantity)
        {
            var cartItem = _cartRepository.GetCartItemById(cartId);
            if (cartItem == null || quantity <= 0)
            {
                return false; // Cart item not found or invalid quantity
            }

            // Get product details to check stock for the new quantity
            var product = _productRepository.GetById(cartItem.ProductId); // <<<--- CORRECTED METHOD NAME HERE
            if (product == null || product.StockQuantity < quantity)
            {
                return false; // Product not found or insufficient stock for the requested quantity
            }

            cartItem.Quantity = quantity;
            _cartRepository.UpdateCartItem(cartItem);
            return true;
        }

        public List<Cart> GetCartDetails(int userId)
        {
            return _cartRepository.GetCartItemsByUserId(userId);
        }

        public void ClearUserCart(int userId)
        {
            _cartRepository.ClearCart(userId);
        }
    }
}