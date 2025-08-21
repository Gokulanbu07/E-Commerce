using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Claims;
using Ecommerce.businesslogic;
using ECommerce.Repository.Models;
using ECommerce.Repository; // Ensure this is present for ApplicationDbContext or User model

namespace NextCart_User_UI.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService _userService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public LoginController(IUserService userService, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _userService = userService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Dashboard()
        {
            // This method will render your views/Login/dashboard.cshtml
            return View();
        }

        // GET: /User/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /User/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _userService.RegisterUserAsync(model);

            if (result.Succeeded)
            {
                TempData["Message"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        // GET: /User/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /User/Login
        [HttpPost]
        [ValidateAntiForgeryToken] // Added for security
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginUserAsync(model);

                if (result.Succeeded)
                {
                    // Find the user by email to get their custom Role property
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }

                        if (user.Role == "ADMIN")
                        {
                            return RedirectToAction("AdminDashboard", "Login");
                        }
                        else if (user.Role == "CUSTOMER")
                        {
                            return RedirectToAction("Index", "User");
                        }
                    }
                    ModelState.AddModelError("", "Failed to retrieve user information after login.");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password."); // Generic message for security
                }
            }
            return View(model);
        }

        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AdminDashboard()
        {
            // Set the current user's role for display in the view
            var user = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUserRole = user?.Role ?? "Unknown"; // '?? "Unknown"' provides a fallback if role is not set
            return View(); // Return AdminDashboard view without a model (as it only contains buttons now)
        }

        // NEW ACTION: RegisteredUsers
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> RegisteredUsers()
        {
            // Fetch all users with the "CUSTOMER" role
            // If you need *all* users (including other admins), you would use _userManager.Users.ToList()
            // but for "Registered Customer List", GetCustomersAsync() is appropriate.
            var customers = await _userService.GetCustomersAsync();
            return View(customers);
        }


        [Authorize(Roles = "CUSTOMER")]
        public IActionResult CustomerDashboard()
        {
            return View();
        }

        // GET: /User/Profile
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                TempData["ErrorMessage"] = "User ID not found. Please log in again.";
                return RedirectToAction("Login");
            }

            var user = await _userService.GetUserProfileAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User profile not found.";
                return RedirectToAction("Login");
            }
            return View(user);
        }

        // POST: /User/UpdateProfile
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(User model)
        {
            // Ensure the user is only updating their own profile
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (model.Id != currentUserId)
            {
                TempData["ErrorMessage"] = "You are not authorized to update this profile.";
                return RedirectToAction("Profile");
            }

            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            var result = await _userService.UpdateUserProfileAsync(model); // Pass the model directly to service

            if (result.Succeeded)
            {
                TempData["Message"] = "Profile updated successfully.";
                return RedirectToAction("Profile");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View("Profile", model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["Message"] = "You have been logged out successfully.";
            return RedirectToAction("Dashboard", "Login");
        }

        // --- New Password Reset Actions ---

        // GET: /User/ForgotPassword
        [HttpGet]
        [AllowAnonymous] // Allow unauthenticated users to access
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /User/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // To prevent enumeration of valid emails, return Failed without specific error
                TempData["Message"] = "If an account exists for that email address, a password reset link has been sent.";
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            var token = await _userService.GeneratePasswordResetTokenAsync(model.Email);
            if (token == null)
            {
                TempData["ErrorMessage"] = "Could not generate password reset token. Please try again.";
                return View(model);
            }
            var resetLink = Url.Action("ResetPassword", "Login",
                new { token = token, email = model.Email }, Request.Scheme);

            // In a real application, you would send this 'resetLink' via email.
            // For now, it's stored in TempData for demonstration/debugging purposes.
            TempData["ResetLink"] = resetLink;

            TempData["Message"] = "If an account exists for that email address, a password reset link has been sent.";
            return RedirectToAction("ForgotPasswordConfirmation");
        }

        // GET: /User/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /User/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token = null, string email = null)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "A valid token and email are required to reset your password.");
            }
            return View(new ResetPasswordViewModel { Email = email, Token = token });
        }

        // POST: /User/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userService.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);

            if (result.Succeeded)
            {
                TempData["Message"] = "Your password has been reset successfully. Please login with your new password.";
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        // GET: /User/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        // TODO: Implement EditUser and DeleteUser Actions as per previous discussion
        // [Authorize(Roles = "ADMIN")]
        // [HttpGet]
        // public async Task<IActionResult> EditUser(string id) { ... }

        // [Authorize(Roles = "ADMIN")]
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> EditUser(User model) { ... }

        // [Authorize(Roles = "ADMIN")]
        // [HttpGet]
        // public async Task<IActionResult> DeleteUser(string id) { ... }

        // [Authorize(Roles = "ADMIN")]
        // [HttpPost, ActionName("DeleteUser")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> DeleteUserConfirmed(string id) { ... }
    }
}