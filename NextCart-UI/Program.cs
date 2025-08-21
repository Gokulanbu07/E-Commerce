
using System;
using Ecommerce.businesslogic; // Assuming this namespace is needed
using ECommerce.businesslogic;
using ECommerce.BusinessLogic;
using ECommerce.Repository;
using ECommerce.Repository.Interfaces;
using ECommerce.Repository.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



// Register ApplicationDbContext - only one registration needed as it's the same database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Password settings (you can customize these)
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings

    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);

    options.LoginPath = "/User/Login";
    options.AccessDeniedPath = "/User/AccessDenied"; // You might want to create an AccessDenied action/view
    options.SlidingExpiration = true;
});

builder.Services.AddScoped<IUserService, UserService>();


#region Product

// Common/User Module Registrations (from original User Program.cs and combined)
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>(); // Present in both, consolidating
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

#endregion

// Shopping Cart Module Registrations (from original User Program.cs)
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value for production scenarios.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Seed roles and admin user
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    string[] roles = { "CUSTOMER", "ADMIN" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Seed Admin User
    var adminUser = await userManager.FindByEmailAsync("admin@example.com"); // Choose an email
    if (adminUser == null)
    {
        var newAdminUser = new User
        {
            UserName = "admin", // Choose a username
            Email = "admin@example.com",
            EmailConfirmed = true,
            Role = "ADMIN" // Set the custom Role property
        };
        var createAdminResult = await userManager.CreateAsync(newAdminUser, "Admin@123"); // Use a strong password
        if (createAdminResult.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdminUser, "ADMIN");
            // Important: Save changes to DB to ensure custom 'Role' property is persisted if not automatically by Identity
            // In most cases, Identity's UserManager.CreateAsync handles saving the user itself.
            // If the custom Role property isn't saving, you might need:
            // await _dbContext.Users.Update(newAdminUser);
            // await _dbContext.SaveChangesAsync();
            // However, Identity should manage the persistence of the User entity itself.
        }
        else
        {
            // Log errors if admin user creation failed
            Console.WriteLine("Error creating admin user:");
            foreach (var error in createAdminResult.Errors)
            {
                Console.WriteLine($"- {error.Description}");
            }
        }
    }
}

// Set the default route to target the User controller as requested.
// If you implement Areas, you will typically add area routes here as well
// before or after the default route, using app.MapControllerRoute(name: "areaName", pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
//LoginUserController is the entry point for the application, so we set it as the default route.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Dashboard}/{id?}"); // Changed default route to User/Login for initial access


//ProductController is the entry point for the application, so we set it as the default route.asdfgdffvgh
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Index}/{id?}");

app.Run();


///Aditi