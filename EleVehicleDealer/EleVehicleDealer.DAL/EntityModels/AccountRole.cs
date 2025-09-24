using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class AccountRole
{
    public int AccountRoleId { get; set; }

    public int AccountId { get; set; }

    public int RoleId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
