using System;
using System.Threading.Tasks;
using ECommerce.BusinessLogic.Interface;
using ECommerce.BusinessLogic.Services;
using ECommerce.Repository;
using ECommerce.Repository.Entity;
using ECommerce.Repository.RepoInterface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NextCart_UI.Filters; // <--- CHANGED: Updated this using statement to match your filter's namespace

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Register the PopulateCartCountFilter globally
        builder.Services.AddControllersWithViews(options =>
        {
            options.Filters.Add<PopulateCartCountFilter>(); // <--- CHANGED: Used your filter's name here
        });
        builder.Services.AddRazorPages();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            options.LoginPath = "/User/Login";
            options.AccessDeniedPath = "/User/AccessDenied";
            options.SlidingExpiration = true;
        });

        builder.Services.AddScoped<IUserService, UserService>();

        #region Product
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        #endregion

        // Shopping Cart Module Registrations
        builder.Services.AddScoped<ICartRepository, CartRepository>();
        builder.Services.AddScoped<ICartService, CartService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IPaymentService, PaymentService>();
        builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();

        // Register your filter as a scoped service so it can inject its dependencies (like ICartService)
        builder.Services.AddScoped<PopulateCartCountFilter>(); // <--- CHANGED: Used your filter's name here

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
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

            var adminUser = await userManager.FindByEmailAsync("admin@example.com");
            if (adminUser == null)
            {
                var newAdminUser = new User
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true,
                    Role = "ADMIN"
                };
                var createAdminResult = await userManager.CreateAsync(newAdminUser, "Admin@123");
                if (createAdminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdminUser, "ADMIN");
                }
                else
                {
                    Console.WriteLine("Error creating admin user:");
                    foreach (var error in createAdminResult.Errors)
                    {
                        Console.WriteLine($"- {error.Description}");
                    }
                }
            }
        }

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Login}/{action=Dashboard}/{id?}");

        // app.MapRazorPages(); // Uncomment if using Identity's default Razor Pages

        app.Run();
    }
}