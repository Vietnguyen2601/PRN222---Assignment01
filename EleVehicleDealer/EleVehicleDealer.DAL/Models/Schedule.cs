using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.Models;

public partial class Schedule
{
    public int ScheduleId { get; set; }

    public int CustomerId { get; set; }

    public int StationCarId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime ScheduleTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual Account Customer { get; set; } = null!;

    public virtual StationCar StationCar { get; set; } = null!;
}
