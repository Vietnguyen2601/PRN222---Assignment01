using EleVehicleDealer.Domain.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleVehicleDealer.BLL.Interfaces
{
    public interface IAccountService
    {
        Task<Account?> LoginAsync(string username, string password);
        Task<bool> ExistsAsync(string username);  
        Task<bool> ExistsEmailAsync(string email);  
        Task RegisterAsync(Account account);

        Task<List<Account>> GetAllAsync();
        Task<Account?> GetByIdAsync(int id);
        Task<int> CreateAsync(Account account);
        Task<int> UpdateAsync(Account account);
        Task<bool> SoftDeleteAsync(int id);
    }
}
