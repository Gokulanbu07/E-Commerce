using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Category> GetTopCategories(int count)
        {
            return _context.Categories.Take(count).ToList();
        }

        public Category? GetCategoryWithProduct(int categoryId)
        {
            return _context.Categories
                .Include(c => c.Products)
                .FirstOrDefault(c => c.categoryId == categoryId);  
        }
    }
}
