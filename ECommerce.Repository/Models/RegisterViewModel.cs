using System.ComponentModel.DataAnnotations;
namespace ECommerce.Repository.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; } = string.Empty; // Initialized

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty; // Initialized

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty; // Initialized

        [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty; // Initialized
    }
}