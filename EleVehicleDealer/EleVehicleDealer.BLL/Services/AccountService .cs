using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.DAL.EntityModels;
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
    }
}
