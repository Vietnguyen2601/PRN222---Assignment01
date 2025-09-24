using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int CustomerId { get; set; }

    public int VehicleId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? FeedbackDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual Account Customer { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
