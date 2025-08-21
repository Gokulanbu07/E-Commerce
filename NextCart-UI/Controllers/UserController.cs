// NextCart_UI/Controllers/UserController.cs
using Ecommerce.businesslogic;
using ECommerce.BusinessLogic;
using ECommerce.Repository; // Needed for Product and Category models in the View
using Microsoft.AspNetCore.Mvc;
using System; // Required for NotImplementedException if you keep the helper local

namespace NextCart_UI.Controllers
{
    public class UserController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public UserController(ICategoryService categoryService, IProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        // Your existing Index action to display all categories
        public IActionResult Index()
        {
            // Changed to GetAllCategories() as previously discussed for the category listing page
            var categories = _categoryService.GetAllCategories();
            return View(categories);
        }

        // Generic action to display products by category ID
        public IActionResult ProductsByCategory(int id)
        {
            var products = _productService.GetProductsByCategory(id);
            var category = _categoryService.GetCategoryById(id);
            ViewBag.CategoryName = category?.Name ?? "Unknown Category";
            return View(products); // Renders Views/User/ProductsByCategory.cshtml
        }

        // --- NEW CATEGORY-SPECIFIC ACTIONS ---

        public IActionResult Fashions()
        {
            int fashionsCategoryId = GetCategoryIdByName("Fashions"); // Get ID from service
            if (fashionsCategoryId == 0) // Category not found
            {
                return NotFound("Fashions category not found.");
            }
            var products = _productService.GetProductsByCategory(fashionsCategoryId);
            ViewBag.CategoryName = "Fashions";
            return View("fashionsView", products); // Renders Views/User/FashionsView.cshtml
        }

        public IActionResult Electronics()
        {
            int electronicsCategoryId = GetCategoryIdByName("Electronics");
            if (electronicsCategoryId == 0) // Category not found
            {
                return NotFound("Electronics category not found.");
            }
            var products = _productService.GetProductsByCategory(electronicsCategoryId);
            ViewBag.CategoryName = "Electronics";
            return View("ElectronicsView", products); // Renders Views/User/ElectronicsView.cshtml
        }

        public IActionResult Groceries()
        {
            int groceriesCategoryId = GetCategoryIdByName("Groceries");
            if (groceriesCategoryId == 0)
            {
                return NotFound("Groceries category not found.");
            }
            var products = _productService.GetProductsByCategory(groceriesCategoryId);
            ViewBag.CategoryName = "Groceries";
            return View("GroceriesView", products); // Renders Views/User/GroceriesView.cshtml
        }

        public IActionResult Furniture()
        {
            int furnitureCategoryId = GetCategoryIdByName("Furniture");
            if (furnitureCategoryId == 0)
            {
                return NotFound("Furniture category not found.");
            }
            var products = _productService.GetProductsByCategory(furnitureCategoryId);
            ViewBag.CategoryName = "Furniture";
            return View("FurnitureView", products); // Renders Views/User/FurnitureView.cshtml
        }

        public IActionResult BeautyProducts()
        {
            int BeautyProductsCategoryId = GetCategoryIdByName("BeautyProducts");
            if (BeautyProductsCategoryId == 0)
            {
                return NotFound("BeautyProducts category not found.");
            }
            var products = _productService.GetProductsByCategory(BeautyProductsCategoryId);
            ViewBag.CategoryName = "BeautyProducts";
            return View("BeautyProducts", products); // Renders Views/User/BEautyProductsView.cshtml
        }


        public IActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                // If search query is empty, redirect to the main categories page
                TempData["SearchMessage"] = "Please enter a search term.";
                return RedirectToAction("Index", "User");
            }

            // Normalize query for case-insensitive comparison and consistent routing
            string normalizedQuery = query.Trim().ToLower();

            // Try to find a direct category match for routing to specific actions first
            switch (normalizedQuery)
            {
                case "fashions":
                    return RedirectToAction("Fashions");
                case "electronics":
                    return RedirectToAction("Electronics");
                case "groceries":
                    return RedirectToAction("Groceries");
                case "furniture":
                    return RedirectToAction("Furniture");
                case "beauty products": // Match "Beauty Products" as it is in your GetCategoryIdByName
                case "beautyproducts": // Also handle without space for common input
                    return RedirectToAction("BeautyProducts");
                // Add more cases here if you have other specific category actions by name
                // case "books and stationary":
                //     return RedirectToAction("BooksAndStationary");
                // case "home appliances":
                //     return RedirectToAction("HomeAppliances");
                default:
                    // If no direct action match by name, try to find by category name for ProductsByCategory action
                    var category = _categoryService.GetCategoryByName(query); // Use original query for lookup
                    if (category != null)
                    {
                        return RedirectToAction("ProductsByCategory", new { id = category.categoryId });
                    }
                    else
                    {
                        // If no category found, redirect to Index with a message
                        TempData["SearchMessage"] = $"No categories or products found for '{query}'. Please try a different search term.";
                        return RedirectToAction("Index", "User");
                    }
            }
        }




        // Helper method using ICategoryService (now directly in service)
        private int GetCategoryIdByName(string categoryName)
        {
            var category = _categoryService.GetCategoryByName(categoryName);
            return category?.categoryId ?? 0; // Return 0 if category not found
        }
    }
}