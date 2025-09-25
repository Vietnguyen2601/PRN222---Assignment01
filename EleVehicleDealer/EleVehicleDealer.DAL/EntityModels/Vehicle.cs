using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class Vehicle
{
    public int VehicleId { get; set; }

    public string Model { get; set; } = null!;

    public string? Type { get; set; }

    public string? Color { get; set; }

    public decimal Price { get; set; }

    public bool Availability { get; set; }

    public int? StationId { get; set; }

    public bool? IsActive { get; set; }

    public int? Quantity { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}
