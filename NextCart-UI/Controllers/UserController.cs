// NextCart_UI/Controllers/UserController.cs
using ECommerce.BusinessLogic.Interface;
using ECommerce.BusinessLogic.Services;
using ECommerce.Repository; // Needed for Product and Category models in the View
using Microsoft.AspNetCore.Mvc;
using System; // Required for NotImplementedException if you keep the helper local

namespace NextCart_UI.Controllers
{
    public class UserController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;

        public UserController(ICategoryService categoryService, IProductService productService, IUserService userService)
        {
            _categoryService = categoryService;
            _productService = productService;
            _userService = userService;
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

        [HttpGet]
        public IActionResult GetCategorySuggestions(string term)
        {
            // term will come from the AJAX request's 'data' parameter
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new object[0]); // Return empty array if term is empty
            }

            var suggestions = _categoryService.GetCategorySuggestions(term)
                                                .Select(c => new { id = c.categoryId, name = c.Name })
                                                .ToList();

            return Json(suggestions);
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
                    return RedirectToAction("FashionsView");
                case "electronics":
                    return RedirectToAction("ElectronicsView");
                case "groceries":
                    return RedirectToAction("GroceriesView");
                case "furniture":
                    return RedirectToAction("FurnitureView");
                case "beauty products": // Match "Beauty Products" as it is in your GetCategoryIdByName
                case "beautyproducts": // Also handle without space for common input
                    return RedirectToAction("BeautyProductsView");
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




        public IActionResult Fashions()
        {
            int fashionsCategoryId = GetCategoryIdByName("Fashions"); // Get ID from service
            if (fashionsCategoryId == 0) // Category not found
            {
                return NotFound("Fashions category not found.");
            }
            var products = _productService.GetProductsByCategory(fashionsCategoryId);
            ViewBag.CategoryName = "Fashions";
            return View("FashionsView", products); // Renders Views/User/FashionsView.cshtml
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
            return View("BeautyProductsView", products); // Renders Views/User/BEautyProductsView.cshtml
        }


        




        // Helper method using ICategoryService (now directly in service)
        private int GetCategoryIdByName(string categoryName)
        {
            var category = _categoryService.GetCategoryByName(categoryName);
            return category?.categoryId ?? 0; // Return 0 if category not found
        }
        public async Task<IActionResult> UserReport()
        {
            var reportData = await _userService.GetAllUsersWithPurchasesAsync();
            return View(reportData);
        }
    }
}