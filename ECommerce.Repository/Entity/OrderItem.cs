using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Repository.Entity
{
    public class OrderItem
    {
        [Key]
        public string OrderItemId { get; set; } = Guid.NewGuid().ToString(); // Primary Key, generated as a GUID

        // Foreign Key to the Order
        [ForeignKey("Order")]
        public string OrderId { get; set; } // Use string to match Order.OrderId type

        // Foreign Key to the Product (assuming you have a Product model and table)
        // Adjust type if your ProductId is not int
        [ForeignKey("Product")] // Assuming a Product model is available
        public int ProductId { get; set; } // Link to your actual Product ID

        [Required]
        [MaxLength(255)]
        public string ProductName { get; set; } // Storing product name for historical purposes

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; } // Quantity * UnitPrice

        // Navigation property to the parent Order
        public virtual Order Order { get; set; }

        // Navigation property to the Product (if you have one)
        // public virtual Product Product { get; set; } // Uncomment if you have a Product model
    }
}