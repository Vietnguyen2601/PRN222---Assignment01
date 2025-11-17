using System.Collections.Generic;
using System.Threading.Tasks;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.DAL.Repositories.Base;

namespace EleVehicleDealer.DAL.Repositories.IRepository
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order?> GetOrderWithDetailsAsync(int id);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<IEnumerable<Order>> GetOrdersWithDetailsAsync();
        Task<IEnumerable<Order>> GetOrdersByStaffAsync(int staffId);
        Task<IEnumerable<Order>> GetOrdersByCustomerAsync(int customerId);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status);
        Task<Order> CreateOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task<bool> SoftDeleteAsync(int id);
    }
}
