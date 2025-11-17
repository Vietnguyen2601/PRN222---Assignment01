using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? ContactNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<AccountRole> AccountRoles { get; set; } = new List<AccountRole>();

    public virtual ICollection<Order> OrderCustomers { get; set; } = new List<Order>();

    public virtual ICollection<Order> OrderStaffs { get; set; } = new List<Order>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
