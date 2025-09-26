using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class Vehicle
{
    public int VehicleId { get; set; }

    public string Model { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Color { get; set; }

    public decimal Price { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<StationCar> StationCars { get; set; } = new List<StationCar>();
}
