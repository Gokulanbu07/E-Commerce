// Ecom.Web/Models/PaymentInputViewModel.cs
using System.ComponentModel.DataAnnotations;
using ECommerce.Repository.Entity; // To use PaymentMethod enum

namespace ECommerce.Repository.Models
{
    public class PaymentInputViewModel
    {
        public string? OrderId { get; set; } // Will be pre-filled from the order summary page

        [Required(ErrorMessage = "Amount is required.")]

        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment method is required.")]
        public PaymentMethod PaymentMethod { get; set; }

        // Properties for Online Payment (Credit Card details) - conditionally required
        [StringLength(19, MinimumLength = 15, ErrorMessage = "Card number must be between 15 and 19 digits.")]
        [DataType(DataType.CreditCard)]
        [Display(Name = "Card Number")]
        public string? CardNumber { get; set; }

        [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Invalid Expiry Date. Use MM/YY format.")]
        [Display(Name = "Expiry Date (MM/YY)")]
        public string? ExpiryDateString { get; set; } // MM/YY or MM/YYYY string

        [StringLength(4, MinimumLength = 3, ErrorMessage = "CVV must be 3 or 4 digits.")]
        [Display(Name = "CVV")]
        public string? CVV { get; set; }
    }
}
