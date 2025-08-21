// ECommerce.BusinessLogic/ICategoryService.cs
using System.Collections.Generic;
using ECommerce.Repository.Entity;
using ECommerce.Repository.Models;
using ECommerce.Repository.RepoInterface;

namespace ECommerce.BusinessLogic.Interface
{
    public interface ICategoryService
    {
        List<Category> GetTopCategories(int count);
        List<Category> GetAllCategories();
        Category GetCategoryById(int categoryId);
        Category GetCategoryByName(string categoryName); // <--- ADD THIS LINE

        IEnumerable<Category> GetCategorySuggestions(string searchTerm);
    }
}