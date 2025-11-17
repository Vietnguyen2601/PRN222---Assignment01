using System.Collections.Generic;
using System.Linq;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.Domain.DTOs.Roles;

namespace EleVehicleDealer.BLL.Mappers
{
    public static class RoleMapper
    {
        public static RoleDto? ToRoleDto(this Role? role)
        {
            if (role == null) return null;

            return new RoleDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                IsActive = role.IsActive
            };
        }

        public static IEnumerable<RoleDto> ToRoleDtos(this IEnumerable<Role>? roles) =>
            roles?.Select(r => r.ToRoleDto()!).Where(r => r != null)! ?? Enumerable.Empty<RoleDto>();
    }
}
