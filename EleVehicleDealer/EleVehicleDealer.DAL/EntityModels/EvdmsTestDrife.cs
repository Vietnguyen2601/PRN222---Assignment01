using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class EvdmsTestDrife
{
    public int TestDriveId { get; set; }

    public int CustomerId { get; set; }

    public int VehicleId { get; set; }

    public int UserId { get; set; }

    public DateTime TestDriveDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual EvdmsCustomer Customer { get; set; } = null!;

    public virtual EvdmsUser User { get; set; } = null!;

    public virtual EvdmsVehicle Vehicle { get; set; } = null!;
}
