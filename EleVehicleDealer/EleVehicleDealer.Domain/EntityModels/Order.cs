using System;
using System.Collections.Generic;

namespace EleVehicleDealer.Domain.EntityModels;

public partial class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public int StationCarId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = null!;

    public int? PromotionId { get; set; }

    public int StaffId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual Account Customer { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Promotion? Promotion { get; set; }

    public virtual Account Staff { get; set; } = null!;

    public virtual StationCar StationCar { get; set; } = null!;
}
