using System.Collections.Generic;
using System.Threading.Tasks;
using EleVehicleDealer.Domain.DTOs.Accounts;

namespace EleVehicleDealer.BLL.Interfaces
{
    public interface IAccountService
    {
        Task<AccountDetailDto?> LoginAsync(string username, string password);
        Task<bool> ExistsAsync(string username);
        Task<bool> ExistsEmailAsync(string email);
        Task<AccountDetailDto?> RegisterAsync(AccountCreateDto account);
        Task<IEnumerable<AccountSummaryDto>> GetAllAsync();
        Task<AccountDetailDto?> GetByIdAsync(int id);
        Task<AccountDetailDto?> CreateAsync(AccountCreateDto account);
        Task<AccountDetailDto?> UpdateAsync(AccountUpdateDto account);
        Task<bool> SoftDeleteAsync(int id);
    }
}
