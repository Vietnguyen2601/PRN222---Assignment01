using System;
using System.Collections.Generic;
using EleVehicleDealer.Domain.DTOs.Orders;
using EleVehicleDealer.Domain.DTOs.Schedules;

namespace EleVehicleDealer.Domain.DTOs
{
    public class CustomerDashboardDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<CustomerOrderDto> PurchasedVehicles { get; set; } = new();
        public List<CustomerScheduleDto> TestDriveSchedules { get; set; } = new();
    }

    public class CustomerOrderDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string VehicleModel { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
    }

    public class CustomerScheduleDto
    {
        public int ScheduleId { get; set; }
        public string VehicleModel { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public string StationLocation { get; set; } = string.Empty;
        public DateTime ScheduleTime { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
