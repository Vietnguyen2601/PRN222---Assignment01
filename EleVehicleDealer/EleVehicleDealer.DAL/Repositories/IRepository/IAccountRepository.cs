using EleVehicleDealer.DAL.EntityModels;
using EleVehicleDealer.DAL.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleVehicleDealer.DAL.Repositories.IRepository
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<Account?> GetByUsernameAsync(string username);
        Task<bool> ExistsAsync(string username);
        Task<bool> ExistsEmailAsync(string email);
        Task AddAsync(Account account);
        Task<Account?> GetByUsernameWithRolesAsync(string username);
    }
}
