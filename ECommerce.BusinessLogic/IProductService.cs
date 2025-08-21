using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Repository;
using System.Collections.Generic;


namespace Ecommerce.businesslogic
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
