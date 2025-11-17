using System;
using System.Collections.Generic;

namespace EleVehicleDealer.Domain.EntityModels;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public int VehicleId { get; set; }

    public int StationId { get; set; }

    public int Quantity { get; set; }

    public DateTime? LastUpdated { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Station Station { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
