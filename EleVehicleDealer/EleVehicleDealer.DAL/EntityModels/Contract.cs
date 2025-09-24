using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class Contract
{
    public int ContractId { get; set; }

    public int OrderId { get; set; }

    public DateTime? ContractDate { get; set; }

    public string? Terms { get; set; }

    public string? Signature { get; set; }

    public string Status { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual Order Order { get; set; } = null!;
}
