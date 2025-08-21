using ECommerce.BusinessLogic.Interface; // This is where ICartService should be
using ECommerce.Repository.Entity;
using ECommerce.Repository.Models;
using ECommerce.Repository.RepoInterface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Added for async operations

namespace ECommerce.BusinessLogic.Services
{
    public class CartService : ICartService // Ensure ICartService is correctly referenced
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        // AddToCartAsync is now the primary method for adding to cart
        public async Task<AddToCartResult> AddToCartAsync(string userId, int productId, int quantity)
        {
            if (quantity <= 0)
            {
                return new AddToCartResult { Success = false, Message = "Quantity must be positive." };
            }

            var product = _productRepository.GetById(productId);
            if (product == null)
            {
                return new AddToCartResult { Success = false, Message = "Product not found." };
            }

            var existingCartItem = _cartRepository.GetCartItemByProductAndUser(userId, productId);

            if (existingCartItem != null)
            {
                int newTotalQuantity = existingCartItem.Quantity + quantity;

                if (newTotalQuantity > product.StockQuantity)
                {
                    return new AddToCartResult { Success = false, Message = $"Cannot add {quantity} more. Only {product.StockQuantity - existingCartItem.Quantity} available in stock." };
                }

                existingCartItem.Quantity = newTotalQuantity;
                _cartRepository.UpdateCartItem(existingCartItem);
            }
            else
            {
                if (product.StockQuantity < quantity)
                {
                    return new AddToCartResult { Success = false, Message = "Not enough stock for this quantity." };
                }

                var newCartItem = new Cart
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity,
                    ProductName = product.Name,
                    UnitPrice = product.Price
                };
                _cartRepository.AddCartItem(newCartItem);
            }

            product.StockQuantity -= quantity;
            _productRepository.Update(product);

            int newCartCount = await GetCartItemCountAsync(userId);

            return new AddToCartResult
            {
                Success = true,
                Message = "Product added to cart successfully!",
                CurrentProductStock = product.StockQuantity,
                NewCartCount = newCartCount
            };
        }

        public bool RemoveFromCart(int cartId)
        {
            var cartItem = _cartRepository.GetCartItemById(cartId);
            if (cartItem == null)
            {
                return false;
            }
            _cartRepository.DeleteCartItem(cartId);
            return true;
        }

        public bool UpdateCartItemQuantity(int cartId, int quantity)
        {
            var cartItem = _cartRepository.GetCartItemById(cartId);
            if (cartItem == null || quantity <= 0)
            {
                return false;
            }

            var product = _productRepository.GetById(cartItem.ProductId);
            if (product == null || product.StockQuantity < quantity)
            {
                return false;
            }

            cartItem.Quantity = quantity;
            _cartRepository.UpdateCartItem(cartItem);
            return true;
        }

        public List<Cart> GetCartDetails(string userId)
        {
            return _cartRepository.GetCartItemsByUserId(userId);
        }

        public void ClearUserCart(string userId)
        {
            _cartRepository.ClearCart(userId);
        }

        public async Task<int> GetCartItemCountAsync(string userId)
        {
            var cartItems = _cartRepository.GetCartItemsByUserId(userId);
            int totalCount = cartItems?.Sum(ci => ci.Quantity) ?? 0;
            return await Task.FromResult(totalCount);
        }
    }

    public class AddToCartResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? CurrentProductStock { get; set; }
        public int NewCartCount { get; set; }
    }
}