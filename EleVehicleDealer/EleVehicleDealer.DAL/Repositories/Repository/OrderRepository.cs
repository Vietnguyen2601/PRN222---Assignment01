using EleVehicleDealer.DAL.EntityModels;
using EleVehicleDealer.DAL.Repositories.IRepository;
using EleVehicleDealer.DAL.Repositories.Base;
using EleVehicleDealer.DAL.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EleVehicleDealer.DAL.DBContext;
using Microsoft.EntityFrameworkCore;

namespace EleVehicleDealer.DAL.Repositories.Repository
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly EvdmsDatabaseContext _context;

        public OrderRepository(EvdmsDatabaseContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Order>  CreateOrderAsync(Order order)
        {
            //Validation 

            // try save
            try
            {
                order.IsActive = true;
                order.OrderDate = DateTime.UtcNow;
                order.Status = "Pending"; // Default status
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                return order;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error creating order in database", ex);
            }
        }

        //public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        //{
        //    return await _context.Orders
        //        .Where(o => o.IsActive ?? false)
        //        .AsNoTracking()
        //        .ToListAsync();
        //}
    }
}
