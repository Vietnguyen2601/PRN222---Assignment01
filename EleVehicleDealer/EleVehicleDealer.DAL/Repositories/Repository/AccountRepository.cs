using EleVehicleDealer.DAL.DBContext;
using EleVehicleDealer.DAL.EntityModels;
using EleVehicleDealer.DAL.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EleVehicleDealer.DAL.Repositories.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly EvdmsDatabaseContext _context;
        public AccountRepository(EvdmsDatabaseContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<EvdmsAccount>> GetAllAsync()
        {
            return await _context.EvdmsAccounts
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<EvdmsAccount?> GetByIdAsync(int id)
        {
            return await _context.EvdmsAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AccountId == id);
        }
        public async Task<IEnumerable<EvdmsAccount>> FindAsync(Expression<Func<EvdmsAccount, bool>> predicate)
        {
            return await _context.EvdmsAccounts
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync();
        }
        public async Task<EvdmsAccount> AddAsync(EvdmsAccount account)
        {
            await _context.EvdmsAccounts.AddAsync(account);
            await _context.SaveChangesAsync();
            return account;
        }
        public async Task<EvdmsAccount> UpdateAsync(EvdmsAccount account)
        {
            _context.EvdmsAccounts.Update(account);
            await _context.SaveChangesAsync();
            return account;
        }
        public async Task DeleteByIdAsync(int id)
        {
            var account = await _context.EvdmsAccounts.FindAsync(id);
            if (account != null)
            {
                _context.EvdmsAccounts.Remove(account);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ExistsAsync(Expression<Func<EvdmsAccount, bool>> predicate)
        {
            return await _context.EvdmsAccounts.AnyAsync(predicate);
        }
        public async Task<EvdmsAccount?> GetByUsernameAsync(string username)
        {
            return await _context.EvdmsAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Username == username);
        }
    }
}
