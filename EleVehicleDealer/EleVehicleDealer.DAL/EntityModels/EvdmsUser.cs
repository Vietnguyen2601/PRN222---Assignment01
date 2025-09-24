using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class EvdmsUser
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string? ContactInfo { get; set; }

    public int RoleId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<EvdmsAccount> EvdmsAccounts { get; set; } = new List<EvdmsAccount>();

    public virtual ICollection<EvdmsCustomer> EvdmsCustomers { get; set; } = new List<EvdmsCustomer>();

    public virtual ICollection<EvdmsFeedback> EvdmsFeedbacks { get; set; } = new List<EvdmsFeedback>();

    public virtual ICollection<EvdmsOrder> EvdmsOrderApprovedByNavigations { get; set; } = new List<EvdmsOrder>();

    public virtual ICollection<EvdmsOrder> EvdmsOrderUsers { get; set; } = new List<EvdmsOrder>();

    public virtual ICollection<EvdmsQuote> EvdmsQuotes { get; set; } = new List<EvdmsQuote>();

    public virtual ICollection<EvdmsReport> EvdmsReports { get; set; } = new List<EvdmsReport>();

    public virtual ICollection<EvdmsTestDrife> EvdmsTestDrives { get; set; } = new List<EvdmsTestDrife>();

    public virtual ICollection<EvdmsVehicleBooking> EvdmsVehicleBookings { get; set; } = new List<EvdmsVehicleBooking>();

    public virtual EvdmsRole Role { get; set; } = null!;
}
