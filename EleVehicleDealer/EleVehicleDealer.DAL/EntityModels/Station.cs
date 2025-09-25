using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class Station
{
    public int StationId { get; set; }

    public string StationName { get; set; } = null!;

    public string? Location { get; set; }

    public string? ContactNumber { get; set; }

    public int? AdminId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Account? Admin { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
}
