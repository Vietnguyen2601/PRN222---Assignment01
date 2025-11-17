using System.Collections.Generic;
using System.Threading.Tasks;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.DAL.Repositories.Base;

namespace EleVehicleDealer.DAL.Repositories.IRepository
{
    public interface IAccountRoleRepository : IGenericRepository<AccountRole>
    {
        Task<IEnumerable<AccountRole>> GetByAccountIdAsync(int accountId);
        Task<AccountRole?> GetAccountRoleAsync(int accountRoleId);
        Task AssignRoleAsync(AccountRole accountRole);
        Task RemoveRoleAsync(int accountRoleId);
        Task<bool> HasRoleAsync(int accountId, int roleId);
    }
}
