using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class EvdmsFeedback
{
    public int FeedbackId { get; set; }

    public int CustomerId { get; set; }

    public int UserId { get; set; }

    public string FeedbackType { get; set; } = null!;

    public string FeedbackContent { get; set; } = null!;

    public DateTime? FeedbackDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual EvdmsCustomer Customer { get; set; } = null!;

    public virtual EvdmsUser User { get; set; } = null!;
}
