using ECommerce.BusinessLogic.Services;
using ECommerce.Repository.Entity;
using ECommerce.Repository.Models;
using ECommerce.Repository.RepoInterface; // Ensure this namespace exists for User entity

// To use the Cart entity model

using System.Collections.Generic;
using System.Threading.Tasks; // Re-added for async methods

namespace ECommerce.BusinessLogic.Interface
{
    public interface ICartService
    {
        // Changed to async and returns AddToCartResult for richer information
        Task<AddToCartResult> AddToCartAsync(string userId, int productId, int quantity);

        bool RemoveFromCart(int cartId);
        bool UpdateCartItemQuantity(int cartId, int quantity);
        List<Cart> GetCartDetails(string userId);
        void ClearUserCart(string userId);
        Task<int> GetCartItemCountAsync(string userId); // Existing async method
    }
}