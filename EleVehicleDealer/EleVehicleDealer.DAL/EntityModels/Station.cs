using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class Station
{
    public int StationId { get; set; }

    public string StationName { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string? ContactNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();

    public virtual ICollection<StationCar> StationCars { get; set; } = new List<StationCar>();
}
