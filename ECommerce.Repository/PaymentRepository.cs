// Ecom.Repo.Implementations/PaymentRepository.cs
// To access ApplicationDbContext
using ECommerce.Repository.Entity;
using ECommerce.Repository.RepoInterface;
using Microsoft.EntityFrameworkCore; // For FirstOrDefaultAsync
using System.Threading.Tasks;

namespace ECommerce.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<Payment?> GetPaymentByIdAsync(string paymentId)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }
    }
}