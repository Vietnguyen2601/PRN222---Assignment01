using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class EvdmsCustomer
{
    public int CustomerId { get; set; }

    public string FullName { get; set; } = null!;

    public string ContactInfo { get; set; } = null!;

    public string? Address { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<EvdmsOrder> EvdmsOrders { get; set; } = new List<EvdmsOrder>();
}
