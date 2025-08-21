using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ECommerce.Repository; // Make sure this is included
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ECommerce.Repository.Models;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the relationship for Cart and Product
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Product)
                .WithMany() // A product can be in many cart items
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // If product is deleted, cart items are deleted

            // You might want to add configuration for UserId if you later introduce a User entity:
            // modelBuilder.Entity<Cart>()
            //    .HasOne(c => c.User)
            //    .WithMany()
            //    .HasForeignKey(c => c.UserId)
            //    .OnDelete(DeleteBehavior.Cascade);

            // Add any other model configurations or seeding your teammate has here.
            // Example seeding (if your teammate does it here):
            // modelBuilder.Entity<Category>().HasData(
            //     new Category { CategoryId = 1, Name = "Electronics" },
            //     new Category { CategoryId = 2, Name = "Fashions" }
            // );
        }
        #endregion

    }
}
