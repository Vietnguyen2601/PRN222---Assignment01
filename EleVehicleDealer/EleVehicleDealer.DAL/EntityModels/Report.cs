using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class Report
{
    public int ReportId { get; set; }

    public string? ReportType { get; set; }

    public DateTime? GeneratedDate { get; set; }

    public string? Data { get; set; }

    public int? AccountId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Account? Account { get; set; }
}
