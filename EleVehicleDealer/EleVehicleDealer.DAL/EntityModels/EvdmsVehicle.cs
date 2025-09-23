using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class EvdmsVehicle
{
    public int VehicleId { get; set; }

    public string ModelName { get; set; } = null!;

    public string? Type { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<EvdmsOrder> EvdmsOrders { get; set; } = new List<EvdmsOrder>();
}
