using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Repository
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int categoryId { get; set; }
        public string? Name { get; set; }
        

        public virtual ICollection<Product>? Products { get; set; } // Navigation property for related products




    }
}
