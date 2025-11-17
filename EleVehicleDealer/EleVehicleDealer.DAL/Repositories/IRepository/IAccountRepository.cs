using System.Collections.Generic;
using System.Threading.Tasks;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.DAL.Repositories.Base;

namespace EleVehicleDealer.DAL.Repositories.IRepository
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<Account?> GetByUsernameAsync(string username);
        Task<Account?> GetByUsernameWithRolesAsync(string username);
        Task<Account?> GetByIdWithRolesAsync(int accountId);
        Task<bool> ExistsAsync(string username);
        Task<bool> ExistsEmailAsync(string email);
        Task AddAsync(Account account);
        Task<IEnumerable<Account>> GetStaffAccountsAsync();
        Task<IEnumerable<Account>> GetCustomerAccountsAsync();
    }
}
