using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Threading.Tasks;
using ECommerce.Repository.Entity;

namespace ECommerce.Repository.Models

{

    // Ecom.Web/Models/PaymentResultViewModel.cs

    public class PaymentResultViewModel

    {

        public string PaymentId { get; set; } = null!;

        public string OrderId { get; set; } = null!;

        public decimal Amount { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public DateTime PaymentDate { get; set; }

        public bool IsSuccess { get; set; }

        public string Message { get; set; } = null!;

    }

}

