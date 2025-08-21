// Ecom.Services.Interfaces/IPaymentService.cs
using ECommerce.Repository.Entity;
using ECommerce.Repository.Models;
using ECommerce.Repository.RepoInterface;
using System.Threading.Tasks;

namespace ECommerce.BusinessLogic.Interface
{
    public interface IPaymentService
    {
        Task<Payment> ProcessCodPaymentAsync(string orderId, decimal amount);
        Task<(Payment payment, string redirectUrl)> ProcessOnlinePaymentAsync(string orderId, decimal amount);
        Task<Payment> HandlePayPalReturnAsync(string paymentId, string token); // Simulate PayPal callback
        Task<Payment?> GetPaymentDetailsAsync(string paymentId);
    }
}