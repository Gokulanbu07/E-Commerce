using System.ComponentModel.DataAnnotations;
namespace ECommerce.Repository.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "UserName must be between 3 and 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "UserName can only contain letters and spaces.")]
        public string Username { get; set; } // Initialized


        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty; // Initialized

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty; // Initialized

        [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty; // Initialized
    }
}