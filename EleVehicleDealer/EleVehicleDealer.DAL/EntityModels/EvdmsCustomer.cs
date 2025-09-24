using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class EvdmsCustomer
{
    public int CustomerId { get; set; }

    public string FullName { get; set; } = null!;

    public string ContactInfo { get; set; } = null!;

    public string? Address { get; set; }

    public string? TransactionHistory { get; set; }

    public int? AssignedStaffId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual EvdmsUser? AssignedStaff { get; set; }

    public virtual ICollection<EvdmsContract> EvdmsContracts { get; set; } = new List<EvdmsContract>();

    public virtual ICollection<EvdmsFeedback> EvdmsFeedbacks { get; set; } = new List<EvdmsFeedback>();

    public virtual ICollection<EvdmsOrder> EvdmsOrders { get; set; } = new List<EvdmsOrder>();

    public virtual ICollection<EvdmsPayment> EvdmsPayments { get; set; } = new List<EvdmsPayment>();

    public virtual ICollection<EvdmsQuote> EvdmsQuotes { get; set; } = new List<EvdmsQuote>();

    public virtual ICollection<EvdmsTestDrife> EvdmsTestDrives { get; set; } = new List<EvdmsTestDrife>();
}
