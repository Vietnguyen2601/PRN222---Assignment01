using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class EvdmsQuote
{
    public int QuoteId { get; set; }

    public int CustomerId { get; set; }

    public int VehicleId { get; set; }

    public int UserId { get; set; }

    public int? PromotionId { get; set; }

    public decimal QuoteAmount { get; set; }

    public DateTime? QuoteDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual EvdmsCustomer Customer { get; set; } = null!;

    public virtual ICollection<EvdmsOrder> EvdmsOrders { get; set; } = new List<EvdmsOrder>();

    public virtual EvdmsPromotion? Promotion { get; set; }

    public virtual EvdmsUser User { get; set; } = null!;

    public virtual EvdmsVehicle Vehicle { get; set; } = null!;
}
