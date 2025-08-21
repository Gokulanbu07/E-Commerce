using System.ComponentModel.DataAnnotations;

namespace ECommerce.Repository.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty; // Initialized

        [Required]
        public string Token { get; set; } = string.Empty; // Initialized

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty; // Initialized

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty; // Initialized
    }
}