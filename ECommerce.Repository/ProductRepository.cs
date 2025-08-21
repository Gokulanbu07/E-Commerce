using ECommerce.Repository.Entity;
using ECommerce.Repository.RepoInterface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Product? GetById(int id)
        {
            return _context.Products.Find(id);
        }

        public IEnumerable<Product> GetAll()
        {
            return _context.Products.ToList();
        }

        public void Add(Product product)
        {
            var catergory = _context.Categories.Find(product.categoryId);
            if (catergory != null)
            {
                product.Category = catergory;
                _context.Products.Add(product);
                _context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Error Occured");
            }
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Product> GetProductsByCategory(int categoryId)
        {
            return _context.Products
                .Where(p => p.categoryId == categoryId)
                .ToList();
        }
    }
}