using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class EvdmsVehicleBooking
{
    public int BookingId { get; set; }

    public int VehicleId { get; set; }

    public int UserId { get; set; }

    public int Quantity { get; set; }

    public DateTime? BookingDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual EvdmsUser User { get; set; } = null!;

    public virtual EvdmsVehicle Vehicle { get; set; } = null!;
}
