using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.Models;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int StationCarId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual StationCar StationCar { get; set; } = null!;
}
