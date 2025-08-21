// Ecom.Repo.Interfaces/IPaymentRepository.cs
using ECommerce.Repository.Entity;
using System.Threading.Tasks;

namespace ECommerce.Repository.RepoInterface
{
    public interface IPaymentRepository
    {
        Task AddPaymentAsync(Payment payment);
        Task UpdatePaymentAsync(Payment payment);
        Task<Payment?> GetPaymentByIdAsync(string paymentId);
    }
}