
using ECommerce.Repository.Models;
using ECommerce.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Repository.Entity;
using ECommerce.BusinessLogic.Interface;

namespace ECommerce.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager; // Corrected capitalization from SigninManager
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;

        public UserService(UserManager<User> userManager,
                                       SignInManager<User> signInManager, // Corrected capitalization
                                       RoleManager<IdentityRole> roleManager,
                                       ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterViewModel model)
        {
            var user = new User { UserName = model.Username, Email = model.Email, Role = "CUSTOMER" };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "CUSTOMER");
                // Ensure the custom Role property is saved to the AspNetUsers table
                // UserManager.CreateAsync usually handles initial persistence of the User entity
                // but explicit SaveChanges might be needed if custom properties are not tracked correctly by default
                // No need for _dbContext.SaveChangesAsync() here if the User entity itself is managed by UserManager
                // and the Role property is part of that entity's state when SaveChanges is implicitly called by CreateAsync.
                // However, for safety or if the custom 'Role' property is missed by Identity's default tracking,
                // you might uncomment the line below, but it's often not necessary.
                // await _dbContext.SaveChangesAsync();
            }

            return result;
        }

        public async Task<SignInResult> LoginUserAsync(LoginViewModel model)
        {
            // Find user by email instead of username
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // To prevent enumeration of valid emails, return Failed without specific error
                return SignInResult.Failed;
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);
            return result;
        }

        public async Task<User> GetUserProfileAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IdentityResult> UpdateUserProfileAsync(User updatedUser)
        {
            var user = await _userManager.FindByIdAsync(updatedUser.Id);

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            // Update allowed properties
            user.UserName = updatedUser.UserName; // User can update their username
            user.Email = updatedUser.Email;       // User can update their email

            // If email or username was changed, Identity's UpdateAsync should handle normalization.
            var result = await _userManager.UpdateAsync(user);

            // No explicit UpdateNormalizedUserNameAsync/UpdateNormalizedEmailAsync needed after UpdateAsync,
            // as UpdateAsync internally calls these if the corresponding properties have changed.

            return result;
        }

        public async Task<List<User>> GetCustomersAsync()
        {
            // Get all users in the "CUSTOMER" role
            return (await _userManager.GetUsersInRoleAsync("CUSTOMER")).ToList();
        }

        // --- New Password Reset Methods ---

        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // To prevent enumeration of valid emails, always return null token if user not found
                return null;
            }
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // To prevent enumeration of valid emails, always return Failed if user not found
                return IdentityResult.Failed(new IdentityError { Description = "An error occurred during password reset." });
            }
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }
        public async Task<List<UserPurchaseViewModel>> GetAllUsersWithPurchasesAsync()
        {
            var usersWithOrders = await _dbContext.Users
                .Include(u => u.Orders)
                    .ThenInclude(o => o.OrderItems)
                .Include(u => u.Orders)
                    .ThenInclude(o => o.Payments)
                .Where(u => u.Orders.Any())
                .ToListAsync();
            var result = usersWithOrders.Select(user => new UserPurchaseViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                ShippingAddress = user.Orders
                    .OrderByDescending(o => o.OrderDate)
                    .FirstOrDefault()?.ShippingAddress ?? "N/A",

                PhoneNumber = user.Orders
                    .OrderByDescending(o => o.OrderDate)
                    .FirstOrDefault()?.ShippingPhoneNumber ?? "N/A",

                Purchases = user.Orders.SelectMany(order => order.OrderItems.Select(item => new PurchaseDetail
                {
                    ProductName = item.ProductName,
                    PurchaseDate = order.OrderDate,
                    PaymentMethod = order.Payments.FirstOrDefault()?.PaymentMethod.ToString() ?? "N/A",
                    TransactionId = order.Payments.FirstOrDefault()?.TransactionId ?? "N/A"
                })).ToList()
            }).ToList();

            return result;
        }
    }
}