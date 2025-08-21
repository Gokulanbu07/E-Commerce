using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Repository.Models
{
    public class UserPurchaseViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ShippingAddress { get; set; }
        public string PhoneNumber { get; set; }
        public List<PurchaseDetail> Purchases { get; set; }
    }
    public class PurchaseDetail
    {
        public string ProductName { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
    }
}
