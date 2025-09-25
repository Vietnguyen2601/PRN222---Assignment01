using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class Promotion
{
    public int PromotionId { get; set; }

    public string PromoCode { get; set; } = null!;

    public decimal? DiscountPercentage { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string? ApplicableTo { get; set; }

    public int? StationId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Station? Station { get; set; }
}
