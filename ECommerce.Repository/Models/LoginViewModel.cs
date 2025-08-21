using System.ComponentModel.DataAnnotations; // Added for [Required]

namespace ECommerce.Repository.Models
{
    public class LoginViewModel
    {
        [Required] // Added [Required] as email/username is mandatory for login
        [EmailAddress] // Assuming Email is the primary login identifier
        public string Email { get; set; } = string.Empty;

        [Required] // Password is mandatory
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}