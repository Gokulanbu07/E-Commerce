using System.ComponentModel.DataAnnotations;

namespace ECommerce.Repository.Models // Keep it in Models for now, though some prefer ViewModels in a separate project
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Address is required.")]
        [StringLength(100, ErrorMessage = "Address cannot exceed 100 characters.")]
        [Display(Name = "Shipping Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        [StringLength(100, ErrorMessage = "State cannot exceed 100 characters.")]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required(ErrorMessage = "Postal Code is required.")]
        [StringLength(8, MinimumLength = 6, ErrorMessage = "Invalid Postal Code")]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [Phone(ErrorMessage = "Invalid Phone Number.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Phone Number")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        // You might want to add properties for payment method selection here later
        // e.g., [Display(Name = "Payment Method")] public string PaymentMethod { get; set; }
    }
}