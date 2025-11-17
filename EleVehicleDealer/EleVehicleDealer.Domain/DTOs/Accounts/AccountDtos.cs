using System;
using System.Collections.Generic;

namespace EleVehicleDealer.Domain.DTOs.Accounts
{
    public class AccountSummaryDto
    {
        public int AccountId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public List<string> Roles { get; set; } = new();
        public bool IsActive { get; set; }
    }

    public class AccountDetailDto
    {
        public int AccountId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public List<string> Roles { get; set; } = new();
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class AccountCreateDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public List<int> RoleIds { get; set; } = new();
    }

    public class AccountUpdateDto
    {
        public int AccountId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public bool IsActive { get; set; }
        public string? Password { get; set; }
        public List<int> RoleIds { get; set; } = new();
    }
}
