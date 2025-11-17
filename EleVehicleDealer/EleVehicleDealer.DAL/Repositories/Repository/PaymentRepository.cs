using EleVehicleDealer.DAL.DBContext;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.DAL.Repositories.Base;
using EleVehicleDealer.DAL.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace EleVehicleDealer.DAL.Repositories.Repository
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(EvdmsDatabaseContext context) : base(context ?? throw new ArgumentNullException(nameof(context)))
        {
        }

        public async Task<IEnumerable<Payment>> GetByOrderIdAsync(int orderId)
        {
            return await _context.Payments
                .Where(p => p.OrderId == orderId && p.IsActive)
                .OrderByDescending(p => p.PaymentDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<decimal> GetTotalPaidAmountAsync(int orderId)
        {
            return await _context.Payments
                .Where(p => p.OrderId == orderId && p.IsActive)
                .SumAsync(p => p.Amount);
        }

        public async Task<IEnumerable<Payment>> GetByStatusAsync(string status)
        {
            return await _context.Payments
                .Where(p => p.IsActive &&
                            p.Status != null &&
                            p.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }
    }
}
