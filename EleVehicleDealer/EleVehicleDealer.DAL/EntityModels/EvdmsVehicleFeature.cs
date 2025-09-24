using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class EvdmsVehicleFeature
{
    public int FeatureId { get; set; }

    public int VehicleId { get; set; }

    public string FeatureName { get; set; } = null!;

    public string? FeatureValue { get; set; }

    public virtual EvdmsVehicle Vehicle { get; set; } = null!;
}
