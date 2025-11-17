using System.Collections.Generic;

namespace EleVehicleDealer.Domain.DTOs.Vehicles
{
    public class VehicleCatalogDto
    {
        public int VehicleId { get; set; }
        public string Model { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Color { get; set; }
        public decimal Price { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class VehicleDetailDto : VehicleCatalogDto
    {
        public int? BatteryCapacity { get; set; }
        public int? Range { get; set; }
        public List<StationInventoryDto> Inventories { get; set; } = new();
    }

    public class StationInventoryDto
    {
        public int StationCarId { get; set; }
        public int StationId { get; set; }
        public string StationName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
    }

    public class VehicleCreateDto
    {
        public string Model { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Color { get; set; }
        public decimal Price { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public int? BatteryCapacity { get; set; }
        public int? Range { get; set; }
    }

    public class VehicleUpdateDto : VehicleCreateDto
    {
        public int VehicleId { get; set; }
        public bool IsActive { get; set; }
    }

    public class VehicleComparisonDto
    {
        public int VehicleId { get; set; }
        public string Model { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public int? BatteryCapacity { get; set; }
        public int? Range { get; set; }
    }
}
