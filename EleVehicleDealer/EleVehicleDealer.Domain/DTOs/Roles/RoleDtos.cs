namespace EleVehicleDealer.Domain.DTOs.Roles
{
    public class RoleDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class RoleCreateDto
    {
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class RoleUpdateDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
