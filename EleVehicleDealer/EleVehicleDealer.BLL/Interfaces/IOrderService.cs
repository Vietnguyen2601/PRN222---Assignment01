using System.Collections.Generic;
using System.Threading.Tasks;
using EleVehicleDealer.Domain.DTOs.Orders;

namespace EleVehicleDealer.BLL.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDetailDto?> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<OrderSummaryDto>> GetAllOrdersAsync();
        Task<OrderDetailDto?> CreateOrderAsync(OrderCreateDto order);
        Task<OrderDetailDto?> UpdateOrderAsync(OrderUpdateDto order);
    }
}
