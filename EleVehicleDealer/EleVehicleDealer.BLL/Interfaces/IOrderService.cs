using EleVehicleDealer.DAL.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleVehicleDealer.BLL.Interfaces
{
    public interface IOrderService
    {

        Task<Order> GetOrderByIdAsync(int orderId);

        Task<IEnumerable<Order>> GetAllOrdersAsync();

        Task<Order> CreateOrderAsync(Order order);

        Task UpdateOrderAsync(Order order);
    }
}
