using EleVehicleDealer.DAL.DBContext;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.DAL.Repositories.Base;
using EleVehicleDealer.DAL.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace EleVehicleDealer.DAL.Repositories.Repository
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(EvdmsDatabaseContext context) : base(context ?? throw new ArgumentNullException(nameof(context)))
        {
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            try
            {
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                return order;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error creating order in database", ex);
            }
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Where(o => o.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersWithDetailsAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.StationCar)
                        .ThenInclude(sc => sc.Vehicle)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.StationCar)
                        .ThenInclude(sc => sc.Station)
                .Include(o => o.Payments)
                .Where(o => o.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderId == id && o.IsActive);
        }

        public async Task<Order?> GetOrderWithDetailsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.StationCar)
                        .ThenInclude(sc => sc.Vehicle)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.StationCar)
                        .ThenInclude(sc => sc.Station)
                .Include(o => o.Payments)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderId == id && o.IsActive);
        }

        public async Task<IEnumerable<Order>> GetOrdersByStaffAsync(int staffId)
        {
            return await _context.Orders
                .Where(o => o.StaffId == staffId && o.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerAsync(int customerId)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId && o.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status)
        {
            return await _context.Orders
                .Where(o => o.IsActive &&
                            o.Status != null &&
                            o.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            try
            {
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error updating order in database", ex);
            }
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
                return false;

            order.IsActive = false;
            order.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
