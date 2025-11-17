using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EleVehicleDealer.DAL.DBContext;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.DAL.Repositories.Base;
using EleVehicleDealer.DAL.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace EleVehicleDealer.DAL.Repositories.Repository
{
    public class AccountRoleRepository : GenericRepository<AccountRole>, IAccountRoleRepository
    {
        public AccountRoleRepository(EvdmsDatabaseContext context) : base(context ?? throw new ArgumentNullException(nameof(context)))
        {
        }

        public async Task<IEnumerable<AccountRole>> GetByAccountIdAsync(int accountId)
        {
            return await _context.AccountRoles
                .Include(ar => ar.Role)
                .Where(ar => ar.AccountId == accountId && ar.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<AccountRole?> GetAccountRoleAsync(int accountRoleId)
        {
            return await _context.AccountRoles
                .AsNoTracking()
                .FirstOrDefaultAsync(ar => ar.AccountRoleId == accountRoleId);
        }

        public async Task AssignRoleAsync(AccountRole accountRole)
        {
            if (accountRole == null)
                throw new ArgumentNullException(nameof(accountRole));

            await _context.AccountRoles.AddAsync(accountRole);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveRoleAsync(int accountRoleId)
        {
            var accountRole = await _context.AccountRoles
                .FirstOrDefaultAsync(ar => ar.AccountRoleId == accountRoleId);

            if (accountRole != null)
            {
                _context.AccountRoles.Remove(accountRole);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasRoleAsync(int accountId, int roleId)
        {
            return await _context.AccountRoles
                .AnyAsync(ar => ar.AccountId == accountId &&
                                ar.RoleId == roleId &&
                                ar.IsActive);
        }
    }
}
