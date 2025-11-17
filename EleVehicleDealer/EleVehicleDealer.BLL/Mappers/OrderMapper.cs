using System.Collections.Generic;
using System.Linq;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.Domain.DTOs.Orders;

namespace EleVehicleDealer.BLL.Mappers
{
    public static class OrderMapper
    {
        public static OrderSummaryDto? ToOrderSummaryDto(this Order? order)
        {
            if (order == null) return null;

            return new OrderSummaryDto
            {
                OrderId = order.OrderId,
                CustomerName = order.Customer?.Username ?? string.Empty,
                StaffName = order.Staff?.Username ?? string.Empty,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                IsActive = order.IsActive
            };
        }

        public static OrderDetailDto? ToOrderDetailDto(this Order? order)
        {
            if (order == null) return null;

            return new OrderDetailDto
            {
                OrderId = order.OrderId,
                CustomerName = order.Customer?.Username ?? string.Empty,
                StaffName = order.Staff?.Username ?? string.Empty,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                IsActive = order.IsActive,
                Items = order.OrderItems.Select(oi => oi.ToOrderItemDto()!).Where(dto => dto != null).ToList(),
                Payments = order.Payments.Select(p => p.ToPaymentDto()!).Where(dto => dto != null).ToList()
            };
        }

        public static OrderItemDto? ToOrderItemDto(this OrderItem? orderItem)
        {
            if (orderItem == null) return null;

            return new OrderItemDto
            {
                OrderItemId = orderItem.OrderItemId,
                StationCarId = orderItem.StationCarId,
                VehicleModel = orderItem.StationCar?.Vehicle?.Model ?? string.Empty,
                StationName = orderItem.StationCar?.Station?.StationName ?? string.Empty,
                Quantity = orderItem.Quantity,
                Price = orderItem.Price
            };
        }

        public static IEnumerable<OrderSummaryDto> ToOrderSummaryDtos(this IEnumerable<Order>? orders) =>
            orders?.Select(o => o.ToOrderSummaryDto()!).Where(dto => dto != null)! ?? Enumerable.Empty<OrderSummaryDto>();
    }
}
