using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.DAL.DBContext;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.DAL.Repositories.IRepository;
using EleVehicleDealer.Domain.DTOs.Orders;
using EleVehicleDealer.BLL.Mappers;

namespace EleVehicleDealer.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly EvdmsDatabaseContext _context;

        public OrderService(IOrderRepository orderRepository, EvdmsDatabaseContext context)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<OrderDetailDto?> CreateOrderAsync(OrderCreateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (!_context.Accounts.Any(a => a.AccountId == dto.CustomerId))
                throw new ArgumentException("Invalid CustomerId.");

            if (!_context.Accounts.Any(a => a.AccountId == dto.StaffId))
                throw new ArgumentException("Invalid StaffId.");

            // Validate Status
            if (string.IsNullOrEmpty(dto.Status))
                throw new ArgumentException("Status is required.");

            if (dto.Items == null || !dto.Items.Any())
                throw new ArgumentException("Order must include at least one item.");

            foreach (var item in dto.Items)
            {
                if (!_context.StationCars.Any(sc => sc.StationCarId == item.StationCarId))
                    throw new ArgumentException($"Invalid StationCarId {item.StationCarId}.");
            }

            var order = new Order
            {
                CustomerId = dto.CustomerId,
                StaffId = dto.StaffId,
                Status = dto.Status,
                OrderDate = DateTime.Now,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsActive = true
            };

            foreach (var item in dto.Items)
            {
                order.OrderItems.Add(new OrderItem
                {
                    StationCarId = item.StationCarId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                });
            }

            order.TotalPrice = order.OrderItems.Sum(i => i.Price * i.Quantity);

            var created = await _orderRepository.CreateOrderAsync(order);
            var detailed = await _orderRepository.GetOrderWithDetailsAsync(created.OrderId);
            return detailed.ToOrderDetailDto();
        }

        public async Task<IEnumerable<OrderSummaryDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetOrdersWithDetailsAsync();
            return orders.ToOrderSummaryDtos();
        }

        public async Task<OrderDetailDto?> GetOrderByIdAsync(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException("Order ID must be greater than zero.", nameof(orderId));

            var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
            return order.ToOrderDetailDto();
        }

        public async Task<OrderDetailDto?> UpdateOrderAsync(OrderUpdateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var existingOrder = await _orderRepository.GetOrderByIdAsync(dto.OrderId);
            if (existingOrder == null)
                throw new ArgumentException($"Order with ID {dto.OrderId} does not exist.");

            existingOrder.Status = dto.Status;
            existingOrder.UpdatedAt = DateTime.Now;

            await _orderRepository.UpdateOrderAsync(existingOrder);
            var reloaded = await _orderRepository.GetOrderWithDetailsAsync(dto.OrderId);
            return reloaded.ToOrderDetailDto();
        }
    }
}
