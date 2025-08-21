using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Repository // Make sure this namespace matches your teammate's entities
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [Required]
        // Assuming a UserId will eventually come from an authentication system.
        // For now, it's just an int.
        public int UserId { get; set; }

        [Required]
        // Foreign Key to the Product entity.
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        // Navigation property to the Product.
        // This assumes your teammate's Product class is in ECommerce.Repository.Entities
        [ForeignKey("ProductId")]
        public Product Product { get; set; } // Ensure ECommerce.Repository.Entities.Product exists and is correct.

        // You will eventually need a User entity and a navigation property for it:
        // [ForeignKey("UserId")]
        // public User User { get; set; }
    }
}