using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class EvdmsOrder
{
    public int OrderId { get; set; }

    public int? CustomerId { get; set; }

    public int? VehicleId { get; set; }

    public int? UserId { get; set; }

    public DateTime? OrderDate { get; set; }

    public int Quantity { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public string? TrackingInfo { get; set; }

    public virtual EvdmsCustomer? Customer { get; set; }

    public virtual EvdmsUser? User { get; set; }

    public virtual EvdmsVehicle? Vehicle { get; set; }
}
