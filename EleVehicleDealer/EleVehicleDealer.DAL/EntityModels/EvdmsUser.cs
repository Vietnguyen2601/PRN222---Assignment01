using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class EvdmsUser
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string? FullName { get; set; }

    public string? ContactInfo { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<EvdmsOrder> EvdmsOrders { get; set; } = new List<EvdmsOrder>();

    public virtual ICollection<EvdmsReport> EvdmsReports { get; set; } = new List<EvdmsReport>();
}
