using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using ECommerce.Repository.Entity;
using ECommerce.Repository.Models;
using ECommerce.Repository.RepoInterface;


namespace ECommerce.BusinessLogic.Interface
{
    public interface IProductService
    {
        List<Product> GetAll();
        Product GetById(int id);
        void Add(Product product);
        void Update(Product product);
        void Delete(int id);

        IEnumerable<Product> GetProductsByCategory(int categoryId);


    }
}
