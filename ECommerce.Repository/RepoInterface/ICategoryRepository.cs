using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Repository.Entity;

namespace ECommerce.Repository.RepoInterface
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetTopCategories(int count);
        Category? GetCategoryWithProduct(int categoryId);
    }
}
