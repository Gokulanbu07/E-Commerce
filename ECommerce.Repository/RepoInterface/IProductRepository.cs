using ECommerce.Repository.Entity;

namespace ECommerce.Repository.RepoInterface
{
    public interface IProductRepository
    {
        Product GetById(int id);
        IEnumerable<Product> GetAll();
        void Add(Product product);
        void Update(Product product);
        void Delete(int id);

        IEnumerable<Product> GetProductsByCategory(int categoryId);

    }
}
