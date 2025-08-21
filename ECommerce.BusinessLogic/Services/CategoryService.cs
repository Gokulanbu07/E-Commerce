// ECommerce.BusinessLogic/CategoryService.cs
using System.Collections.Generic;
using System.Linq;
using ECommerce.BusinessLogic.Interface;
using ECommerce.Repository;
using ECommerce.Repository.Entity;
using ECommerce.Repository.Models;
namespace ECommerce.BusinessLogic.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Category> GetTopCategories(int count)
        {
            // Assuming you want the top 'count' categories, perhaps ordered by name or popularity
            return _context.Categories.OrderBy(c => c.Name).Take(count).ToList();
        }

        public List<Category> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        public Category GetCategoryById(int categoryId)
        {
            return _context.Categories.FirstOrDefault(c => c.categoryId == categoryId);
        }

        // <--- ADD THIS METHOD
        public Category GetCategoryByName(string categoryName)
        {
            // Use StringComparison.OrdinalIgnoreCase for case-insensitive comparison
            return _context.Categories
                            .FirstOrDefault(c => c.Name != null && c.Name.ToLower() == categoryName.ToLower());
        }

        // --- NEW IMPLEMENTATION FOR SUGGESTIONS ---
        public IEnumerable<Category> GetCategorySuggestions(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<Category>(); // Return empty list if no search term
            }

            // Case-insensitive search using StartsWith
            var suggestions = _context.Categories
                                      .Where(c => c.Name.ToLower().StartsWith(searchTerm.ToLower()))
                                      .Select(c => new Category { categoryId = c.categoryId, Name = c.Name }) // Project to a new Category to avoid tracking if not needed, or just select anonymous type
                                      .Take(10) // Limit to a reasonable number of suggestions
                                      .ToList();

            return suggestions;
        }
    }
}