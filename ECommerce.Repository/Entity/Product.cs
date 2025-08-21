using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repository.Entity
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int productId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }


        [ForeignKey("CategoryId")]
        public int categoryId { get; set; }

        public virtual Category? Category { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
    }
}
