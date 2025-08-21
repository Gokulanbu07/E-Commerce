using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// (assuming Product.cs is also in ECommerce.Repository.Models)

namespace ECommerce.Repository.Entity
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; } // Keep int if this is your primary key for the Cart table itself

        [Required]
        // !!! IMPORTANT CHANGE: Use string for UserId to match IdentityUser.Id !!!
        public string UserId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        // !!! RECOMMENDED ADDITION: Denormalize ProductName and UnitPrice !!!
        // This makes it easier for OrderService to access these details without always loading the Product.
        [Required]
        [MaxLength(255)]
        public string ProductName { get; set; } // Add this line

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; } // Add this line

        // Navigation property to the Product.
        [ForeignKey("ProductId")]
        public Product Product { get; set; } // Ensure ECommerce.Repository.Models.Product exists and is correct.

        // !!! IMPORTANT ADDITION: Navigation property to the User !!!
        // This explicitly defines the relationship from Cart to User.
        [ForeignKey("UserId")]
        public User User { get; set; } // Ensure ECommerce.Repository.User exists and is correct.
    }
}