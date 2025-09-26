using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class StaffRevenue
{
    public int StaffRevenueId { get; set; }

    public int StaffId { get; set; }

    public DateTime RevenueDate { get; set; }

    public decimal TotalRevenue { get; set; }

    public decimal? Commission { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual Account Staff { get; set; } = null!;
}
