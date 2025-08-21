using System;
using System.Collections.Generic;
using System.Linq; // Added for .ToList() and .Where()
using System.Text;
using System.Threading.Tasks;
using Ecommerce.businesslogic; // Namespace for IProductService
using ECommerce.Repository; // Namespace for Product, Category, ApplicationDbContext
using ECommerce.Repository.Interfaces; // Namespace for IProductRepository

namespace ECommerce.BusinessLogic
{
    public class ProductService : IProductService
    {
        // Renamed _repo to _productRepository for clarity
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context; // Field for DbContext, primarily for direct queries if needed

        // Constructor: Inject IProductRepository and ApplicationDbContext
        public ProductService(IProductRepository productRepository, ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }

        public List<Product> GetAll()
        {
            // Delegate to the repository
            return _productRepository.GetAll().ToList();
        }

        public Product GetById(int id)
        {
            // Delegate to the repository
            return _productRepository.GetById(id);
        }

        public void Add(Product product)
        {
            // Delegate to the repository
            _productRepository.Add(product);
        }

        public void Update(Product product)
        {
            // Delegate to the repository
            _productRepository.Update(product);
        }

        public void Delete(int id)
        {
            // Delegate to the repository
            _productRepository.Delete(id);
        }

        public IEnumerable<Product> GetProductsByCategory(int categoryId)
        {
            // This line assumes you've added GetProductsByCategory to IProductRepository
            return _productRepository.GetProductsByCategory(categoryId);
        }

       
        public List<Category> GetAllCategories()
        {
            return _context.Categories.ToList();
        }
    }
}