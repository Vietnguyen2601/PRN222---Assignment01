using System;

namespace EleVehicleDealer.Domain.DTOs.Cart
{
    public class CartItemDto
    {
        public int VehicleId { get; set; }
        public string Model { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageName { get; set; } = "default.jpg"; // Thêm property ?? l?u tên ?nh
        public decimal TotalPrice => Price * Quantity;
    }
}
