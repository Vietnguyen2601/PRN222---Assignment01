using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class EvdmsContract
{
    public int ContractId { get; set; }

    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public string? ContractDetails { get; set; }

    public DateTime? ContractDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual EvdmsCustomer Customer { get; set; } = null!;

    public virtual EvdmsOrder Order { get; set; } = null!;
}
