using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // For [Key] attribute
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Repository.Entity
{
    public class Order
    {
        [Key]
        public string OrderId { get; set; } = Guid.NewGuid().ToString(); // Primary Key, generated as a GUID

        // Foreign Key to AspNetUsers table
        [ForeignKey("User")] // Assuming your User model is named 'User' and maps to AspNetUsers
        public string UserId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow; // When the order was placed (UTC)

        [Required]
        [Column(TypeName = "decimal(18,2)")] // Ensure correct decimal precision for currency
        public decimal TotalAmount { get; set; }

        [Required]
        [MaxLength(50)] // e.g., "Pending", "Processing", "Shipped", "Delivered", "Cancelled"
        public string OrderStatus { get; set; } = "Pending"; // Default status

        [Required]
        [MaxLength(500)]
        public string ShippingAddress { get; set; } // User-entered shipping address

        [Required]
        [MaxLength(100)]
        public string ShippingCity { get; set; }

        [Required]
        [MaxLength(100)]
        public string ShippingState { get; set; }

        [Required]
        [MaxLength(20)]
        public string ShippingPostalCode { get; set; }

        [Required]
        [MaxLength(20)]
        public string ShippingPhoneNumber { get; set; } // User-entered phone number

        [Required]
        [MaxLength(256)] // Matches Identity's email max length
        public string CustomerEmail { get; set; } // Automatically fetched from AspNetUsers

        // Navigation property to the User who placed the order
        public virtual User User { get; set; }

        // Navigation property for order items
        public virtual ICollection<OrderItem> OrderItems { get; set; }

        // Navigation property for payment (one-to-one or one-to-many depending on if order can have multiple payments)
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>(); // Initialize to avoid null reference
    }
}