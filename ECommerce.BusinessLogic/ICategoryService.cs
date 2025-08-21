// ECommerce.BusinessLogic/ICategoryService.cs
using System.Collections.Generic;
using ECommerce.Repository;

namespace ECommerce.BusinessLogic
{
    public interface ICategoryService
    {
        List<Category> GetTopCategories(int count);
        List<Category> GetAllCategories();
        Category GetCategoryById(int categoryId);
        Category GetCategoryByName(string categoryName); // <--- ADD THIS LINE
    }
}