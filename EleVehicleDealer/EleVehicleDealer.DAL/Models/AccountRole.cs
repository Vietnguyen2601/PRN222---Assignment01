using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.Models;

public partial class AccountRole
{
    public int AccountRoleId { get; set; }

    public int AccountId { get; set; }

    public int RoleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
