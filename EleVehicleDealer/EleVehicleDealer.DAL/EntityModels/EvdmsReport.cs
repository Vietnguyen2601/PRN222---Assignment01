using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class EvdmsReport
{
    public int ReportId { get; set; }

    public int? UserId { get; set; }

    public string? ReportType { get; set; }

    public string? ReportData { get; set; }

    public DateTime? GeneratedAt { get; set; }

    public virtual EvdmsUser? User { get; set; }
}
