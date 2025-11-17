using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.Domain.EntityModels;
using EleVehicleDealer.DAL.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleVehicleDealer.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<Account?> LoginAsync(string username, string password)
        {
            var account = await _accountRepository.GetByUsernameWithRolesAsync(username);
            if (account == null) return null;

            // ⚠️ Hiện tại bạn đang so sánh plain text, 
            // thực tế nên hash (BCrypt / HMACSHA512)
            if (account.Password != password) return null;

            return account;
        }

        public async Task<bool> ExistsAsync(string username)
        {
            return await _accountRepository.ExistsAsync(username);
        }

        public async Task<bool> ExistsEmailAsync(string email)
        {
            return await _accountRepository.ExistsEmailAsync(email);
        }

        public async Task RegisterAsync(Account account)
        {
            await _accountRepository.AddAsync(account);
        }

        public async Task<List<Account>> GetAllAsync()
        {
            return await _accountRepository.GetAllAsync();
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            return await _accountRepository.GetByIdAsync(id);
        }

        public async Task<int> CreateAsync(Account account)
        {
            account.CreatedAt = DateTime.Now;
            account.IsActive = true;
            return await _accountRepository.CreateAsync(account);
        }

        public async Task<int> UpdateAsync(Account account)
        {
            var dbAccount = await _accountRepository.GetByIdAsync(account.AccountId);
            if (dbAccount == null) return 0;

            dbAccount.Username = account.Username;
            dbAccount.Email = account.Email;
            dbAccount.ContactNumber = account.ContactNumber;
            dbAccount.IsActive = account.IsActive;
            dbAccount.UpdatedAt = DateTime.Now;

            // Nếu password không rỗng thì mới cập nhật
            if (!string.IsNullOrWhiteSpace(account.Password))
                dbAccount.Password = account.Password;

            return await _accountRepository.UpdateAsync(dbAccount);
        }

        // Soft delete: chỉ đổi IsActive = false
        public async Task<bool> SoftDeleteAsync(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null) return false;

            account.IsActive = false;
            account.UpdatedAt = DateTime.Now;

            await _accountRepository.UpdateAsync(account);
            return true;
        }
    }
}
