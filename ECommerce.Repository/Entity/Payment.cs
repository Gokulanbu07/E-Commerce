// ECommerce.Repository/Payment.cs

using System;

using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Repository.Entity

{

    public enum PaymentStatus

    {

        Pending,

        COMPLETED, // Changed to uppercase for consistency with common payment gateway statuses

        FAILED,

        REFUNDED,

        COD // Cash On Delivery

    }

    public enum PaymentMethod

    {

        Online,

        COD, // Cash On Delivery

        CreditCard, // Specific online method

        DebitCard,  // Specific online method

        NetBanking, // Specific online method

        UPI        // Specific online method

        // Specific online method

    }

    public class Payment

    {

        [Key]

        public string PaymentId { get; set; } = Guid.NewGuid().ToString(); // Primary Key, generated as GUID

        // Foreign Key to the Order

        [ForeignKey("Order")]

        [Required]

        public string OrderId { get; set; } = null!; // Must be associated with an Order

        [Required]

        [Column(TypeName = "decimal(18,2)")] // Ensure correct decimal precision for currency

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow; // When the payment was processed (UTC)

        [Required]

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending; // Default status

        [Required]

        public PaymentMethod PaymentMethod { get; set; }

        // Transaction ID from payment gateway (nullable)

        [MaxLength(255)]

        public string? TransactionId { get; set; }

        // Any additional details from the payment gateway (e.g., response code, message)

        [MaxLength(1000)]

        public string? GatewayResponse { get; set; }

        // Navigation property to the Order

        public virtual Order Order { get; set; } = null!; // Required navigation property

    }

}
