using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
namespace ECommerce.Repository.Entity
{
    public class User : IdentityUser
    {
        public string Role { get; set; } //customer or admin
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}