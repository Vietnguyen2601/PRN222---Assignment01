using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class EvdmsPayment
{
    public int PaymentId { get; set; }

    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public string PaymentType { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? InstallmentPlan { get; set; }

    public decimal? DebtAmount { get; set; }

    public string Status { get; set; } = null!;

    public virtual EvdmsCustomer Customer { get; set; } = null!;

    public virtual EvdmsOrder Order { get; set; } = null!;
}
