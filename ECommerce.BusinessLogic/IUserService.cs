
using ECommerce.Repository;
using ECommerce.Repository.Models;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.businesslogic
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterViewModel model);
        Task<SignInResult> LoginUserAsync(LoginViewModel model);
        Task<User> GetUserProfileAsync(string userId); // Ensure 'User' is defined in NextCart_User_UI.Models
        Task<IdentityResult> UpdateUserProfileAsync(User updatedUser);
        Task<List<User>> GetCustomersAsync();
        Task<string> GeneratePasswordResetTokenAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword);
    }
}