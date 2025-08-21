using ECommerce.BusinessLogic; // Assuming IProductService and ICategoryService are here
using ECommerce.Repository; // For Product model
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting; // Needed for IWebHostEnvironment
using System.IO; // Needed for Path, FileStream
using System;
using Ecommerce.businesslogic; // Needed for Guid

namespace NextCart_User_UI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService; // Changed to non-nullable if injected
        private readonly ICategoryService _categoryService; // Add this
        private readonly IWebHostEnvironment _webHostEnvironment; // Add this

        // Update constructor to inject new services
        public ProductController(IProductService productService, ICategoryService categoryService, IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _categoryService = categoryService; // Assign injected service
            _webHostEnvironment = webHostEnvironment; // Assign injected service
        }

        public IActionResult Index()
        {
            var products = _productService.GetAll(); // Removed null check since injected
            return View(products);
        }

        public IActionResult Details(int id)
        {
            // Removed null check for _productService as it's injected and shouldn't be null
            var product = _productService.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public IActionResult Create()
        {
            // Get categories from ICategoryService
            ViewBag.Categories = new SelectList(_categoryService.GetAllCategories(), "categoryId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Good practice for POST actions
        public async Task<IActionResult> Create(Product product, IFormFile? productImage) // Add IFormFile parameter
        {
            // Removed null check for _productService as it's injected and shouldn't be null

            // Populate categories for dropdown in case ModelState is invalid
            ViewBag.Categories = new SelectList(_categoryService.GetAllCategories(), "categoryId", "Name", product.categoryId);

            if (ModelState.IsValid)
            {
                if (productImage != null && productImage.Length > 0)
                {
                    // Define the path where images will be saved within wwwroot
                    // For example: wwwroot/images/products/
                    string uploadsFolder = "C:\\Users\\2407103\\source\\repos\\ECommerce.UI\\ECommerce.UI\\NextCart-UI\\wwwroot\\ProductImages";
 

                    // Ensure the directory exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Generate a unique file name to prevent overwrites and provide security
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + productImage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the image file to the server
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await productImage.CopyToAsync(fileStream); // Use await for async operation
                    }

                    // Store the relative URL/path in the database
                    product.ImageUrl = "/ProductImages/" + uniqueFileName;
                }
                else
                {
                    // Optional: Set a default placeholder image if no image is uploaded
                    // Make sure "/images/placeholder.png" exists in your wwwroot/images folder
                    product.ImageUrl = "/ProductImages/Watch.jpg";
                }

                _productService.Add(product); // Add the product to the database
                return RedirectToAction("Index");
            }
            // If ModelState is not valid, return to the view with the current product and validation errors
            return View(product);
        }

        public IActionResult Edit(int id)
        {
            // Removed null check for _productService
            var product = _productService.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            // Populate categories for the dropdown in Edit view
            ViewBag.Categories = new SelectList(_categoryService.GetAllCategories(), "categoryId", "Name", product.categoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? productImage) // Add IFormFile parameter
        {
            // Removed null check for _productService

            if (id != product.productId)
            {
                return NotFound();
            }

            // Populate categories for dropdown in case ModelState is invalid
            ViewBag.Categories = new SelectList(_categoryService.GetAllCategories(), "categoryId", "Name", product.categoryId);

            if (ModelState.IsValid)
            {
                // Retrieve the existing product to check its current image
                var existingProduct = _productService.GetById(id);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                if (productImage != null && productImage.Length > 0)
                {
                    // Delete old image if it exists and is not the default placeholder
                    if (!string.IsNullOrEmpty(existingProduct.ImageUrl) && existingProduct.ImageUrl != "/ProductImages/Watch.jpg")
                    {
                        string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingProduct.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    string uploadsFolder = "C:\\Users\\2407103\\source\\repos\\ECommerce.UI\\ECommerce.UI\\NextCart-UI\\wwwroot\\ProductImages";

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + productImage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await productImage.CopyToAsync(fileStream);
                    }
                    product.ImageUrl = "/ProductImages/" + uniqueFileName;
                }
                else
                {
                    // If no new image is uploaded, retain the existing one
                    product.ImageUrl = existingProduct.ImageUrl;
                }

                // Update the product properties from the form, but ensure productId is correct
                // This assumes your Add/Update methods update all properties.
                // A better approach would be to update properties on the existingProduct object
                // then save existingProduct.
                // For simplicity now, assuming product.ImageUrl is the only field that needs careful handling.
                // If you update all fields, ensure to load existing product, update its properties and then save.
                // Example for updating all properties:
                existingProduct.categoryId = product.categoryId;
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.StockQuantity = product.StockQuantity;
                existingProduct.ImageUrl = product.ImageUrl; // This line is crucial

                _productService.Update(existingProduct); // Pass the updated existingProduct
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public IActionResult Delete(int id)
        {
            // Removed null check for _productService
            var product = _productService.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // Removed null check for _productService

            var product = _productService.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            // Optional: Delete the image file from the server when product is deleted
            if (!string.IsNullOrEmpty(product.ImageUrl) && product.ImageUrl != "/ProductImages/Watch.jpg")
            {
                string uploadsFolder = "C:\\Users\\2407103\\source\\repos\\ECommerce.UI\\ECommerce.UI\\NextCart-UI\\wwwroot\\ProductImages";
            }

            _productService.Delete(id);
            return RedirectToAction("Index");
        }
    }
}