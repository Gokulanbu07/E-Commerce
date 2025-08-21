using System.ComponentModel.DataAnnotations;

namespace ECommerce.Repository.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}