using System.Collections.Generic;
using System.Threading.Tasks;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.DAL.Repositories.Base;

namespace EleVehicleDealer.DAL.Repositories.IRepository
{
    public interface IOrderItemRepository : IGenericRepository<OrderItem>
    {
        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId);
        Task AddRangeAsync(IEnumerable<OrderItem> items);
        Task RemoveByOrderIdAsync(int orderId);
    }
}
