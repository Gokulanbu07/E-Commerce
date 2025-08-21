// ECommerce.BusinessLogic/CategoryService.cs
using System.Collections.Generic;
using System.Linq;
using ECommerce.Repository; // For Category, ApplicationDbContext

namespace ECommerce.BusinessLogic
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
    }
}