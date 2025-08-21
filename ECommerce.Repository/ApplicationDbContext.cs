using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ECommerce.Repository.Entity;

namespace ECommerce.Repository
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        #region Shoppingcart
        // ADD THIS LINE FOR YOUR CART MODULE:
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the relationship for Cart and Product
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Product)
                .WithMany() // A product can be in many cart items
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // If product is deleted, cart items are deleted



            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Adjust DeleteBehavior as needed (e.g., .Restrict)

            // For Order to Payment
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Payments)
                .WithOne(p => p.Order)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Adjust DeleteBehavior as needed

            // For User to Order (one user can have many orders)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Or .Restrict if you want to prevent user deletion if they have orders


            modelBuilder.Entity<Payment>()
    .Property(p => p.PaymentStatus)
    .HasConversion<string>();

            // Configure the precision and scale for the 'Amount' property of the 'Payment' entity
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2); // Defines the column as decimal(18,2) in SQL Server

            // Configure enum to string conversion for PaymentMethod
            modelBuilder.Entity<Payment>()
                .Property(p => p.PaymentMethod)
                .HasConversion<string>();
        }
        #endregion





    }
}
