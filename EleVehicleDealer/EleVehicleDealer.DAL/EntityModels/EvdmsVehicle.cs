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

    public string? Configuration { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<EvdmsOrder> EvdmsOrders { get; set; } = new List<EvdmsOrder>();

    public virtual ICollection<EvdmsQuote> EvdmsQuotes { get; set; } = new List<EvdmsQuote>();

    public virtual ICollection<EvdmsTestDrife> EvdmsTestDrives { get; set; } = new List<EvdmsTestDrife>();

    public virtual ICollection<EvdmsVehicleBooking> EvdmsVehicleBookings { get; set; } = new List<EvdmsVehicleBooking>();

    public virtual ICollection<EvdmsVehicleFeature> EvdmsVehicleFeatures { get; set; } = new List<EvdmsVehicleFeature>();
}
