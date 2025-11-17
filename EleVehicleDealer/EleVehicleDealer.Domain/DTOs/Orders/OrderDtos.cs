using System;
using System.Collections.Generic;
using EleVehicleDealer.Domain.DTOs.Payments;

namespace EleVehicleDealer.Domain.DTOs.Orders
{
    public class OrderItemDto
    {
        public int OrderItemId { get; set; }
        public int StationCarId { get; set; }
        public string VehicleModel { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderSummaryDto
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string StaffName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class OrderDetailDto : OrderSummaryDto
    {
        public List<OrderItemDto> Items { get; set; } = new();
        public List<PaymentDto> Payments { get; set; } = new();
    }

    public class OrderCreateItemDto
    {
        public int StationCarId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderCreateDto
    {
        public int CustomerId { get; set; }
        public int StaffId { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<OrderCreateItemDto> Items { get; set; } = new();
    }

    public class OrderUpdateDto
    {
        public int OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
