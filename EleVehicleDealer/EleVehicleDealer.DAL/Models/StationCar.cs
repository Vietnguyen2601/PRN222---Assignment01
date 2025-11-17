using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.Models;

public partial class StationCar
{
    public int StationCarId { get; set; }

    public int VehicleId { get; set; }

    public int StationId { get; set; }

    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual Station Station { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
