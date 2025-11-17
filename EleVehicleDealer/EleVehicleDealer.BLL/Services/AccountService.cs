using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.DAL.Repositories.IRepository;
using EleVehicleDealer.Domain.DTOs.Accounts;
using EleVehicleDealer.BLL.Mappers;

namespace EleVehicleDealer.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<AccountDetailDto?> LoginAsync(string username, string password)
        {
            var account = await _accountRepository.GetByUsernameWithRolesAsync(username);
            if (account == null) return null;

            // ⚠️ Hiện tại bạn đang so sánh plain text, 
            // thực tế nên hash (BCrypt / HMACSHA512)
            if (account.Password != password) return null;

            return account.ToAccountDetailDto();
        }

        public Task<bool> ExistsAsync(string username) => _accountRepository.ExistsAsync(username);

        public Task<bool> ExistsEmailAsync(string email) => _accountRepository.ExistsEmailAsync(email);

        public async Task<AccountDetailDto?> RegisterAsync(AccountCreateDto dto)
        {
            var entity = MapToAccount(dto);
            await _accountRepository.AddAsync(entity);
            var created = await _accountRepository.GetByIdWithRolesAsync(entity.AccountId);
            return created.ToAccountDetailDto();
        }

        public async Task<IEnumerable<AccountSummaryDto>> GetAllAsync()
        {
            var accounts = await _accountRepository.GetAllAsync();
            return accounts.ToAccountSummaryDtos();
        }

        public async Task<AccountDetailDto?> GetByIdAsync(int id)
        {
            var account = await _accountRepository.GetByIdWithRolesAsync(id);
            return account.ToAccountDetailDto();
        }

        public async Task<AccountDetailDto?> CreateAsync(AccountCreateDto dto)
        {
            var entity = MapToAccount(dto);
            entity.CreatedAt = DateTime.Now;
            await _accountRepository.CreateAsync(entity);
            var created = await _accountRepository.GetByIdWithRolesAsync(entity.AccountId);
            return created.ToAccountDetailDto();
        }

        public async Task<AccountDetailDto?> UpdateAsync(AccountUpdateDto dto)
        {
            var dbAccount = await _accountRepository.GetByIdAsync(dto.AccountId);
            if (dbAccount == null) return null;

            dbAccount.Username = dto.Username;
            dbAccount.Email = dto.Email;
            dbAccount.ContactNumber = dto.ContactNumber;
            dbAccount.IsActive = dto.IsActive;
            dbAccount.UpdatedAt = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                dbAccount.Password = dto.Password!;
            }

            await _accountRepository.UpdateAsync(dbAccount);
            var accountWithRoles = await _accountRepository.GetByIdWithRolesAsync(dto.AccountId);
            return accountWithRoles.ToAccountDetailDto();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null) return false;

            account.IsActive = false;
            account.UpdatedAt = DateTime.Now;

            await _accountRepository.UpdateAsync(account);
            return true;
        }

        private static Account MapToAccount(AccountCreateDto dto)
        {
            var account = new Account
            {
                Username = dto.Username,
                Password = dto.Password,
                Email = dto.Email,
                ContactNumber = dto.ContactNumber,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            if (dto.RoleIds != null && dto.RoleIds.Any())
            {
                foreach (var roleId in dto.RoleIds)
                {
                    account.AccountRoles.Add(new AccountRole
                    {
                        RoleId = roleId,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    });
                }
            }

            return account;
        }
    }
}
