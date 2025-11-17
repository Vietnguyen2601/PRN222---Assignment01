using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.Models;

public partial class Vehicle
{
    public int VehicleId { get; set; }

    public string Model { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Color { get; set; }

    public decimal Price { get; set; }

    public string Manufacturer { get; set; } = null!;

    public int? BatteryCapacity { get; set; }

    public int? Range { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<StationCar> StationCars { get; set; } = new List<StationCar>();
}
