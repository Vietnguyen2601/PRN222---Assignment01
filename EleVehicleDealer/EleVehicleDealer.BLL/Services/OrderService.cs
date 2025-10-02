using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.DAL.DBContext;
using EleVehicleDealer.DAL.EntityModels;
using EleVehicleDealer.DAL.Repositories.IRepository;
using EleVehicleDealer.DAL.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleVehicleDealer.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly EvdmsDatabaseContext _context = new EvdmsDatabaseContext();

        public OrderService(IOrderRepository orderRepository, IVehicleRepository vehicleRepository, EvdmsDatabaseContext context)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            // Validate CustomerId
            if (!_context.Accounts.Any(a => a.AccountId == order.CustomerId))
                throw new ArgumentException("Invalid CustomerId.");

            // Validate StationCarId
            if (!_context.StationCars.Any(sc => sc.StationCarId == order.StationCarId))
                throw new ArgumentException("Invalid StationCarId.");

            // Validate PromotionId (if provided)
            if (order.PromotionId.HasValue && !_context.Promotions.Any(p => p.PromotionId == order.PromotionId))
                throw new ArgumentException("Invalid PromotionId.");

            // Validate Status
            if (string.IsNullOrEmpty(order.Status))
                throw new ArgumentException("Status is required.");

            // Set default values
            order.OrderDate = DateTime.Now;
            order.CreatedAt = DateTime.Now;
            order.UpdatedAt = DateTime.Now;

            // Call repository to save the order
            return await _orderRepository.CreateOrderAsync(order);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException("Order ID must be greater than zero.", nameof(orderId));

            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} was not found.");

            return order;
        }

        public async Task UpdateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            // Validate if the order exists in the database
            var existingOrder = await _orderRepository.GetOrderByIdAsync(order.OrderId);
            if (existingOrder == null)
                throw new ArgumentException($"Order with ID {order.OrderId} does not exist.");

            // Update the status and timestamps
            existingOrder.Status = order.Status;
            existingOrder.UpdatedAt = DateTime.Now;

            // Save changes to the database
            await _orderRepository.UpdateOrderAsync(existingOrder);
        }
    }
}
