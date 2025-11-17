using EleVehicleDealer.DAL.DBContext;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.DAL.Repositories.Base;
using EleVehicleDealer.DAL.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace EleVehicleDealer.DAL.Repositories.Repository
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(EvdmsDatabaseContext context) : base(context ?? throw new ArgumentNullException(nameof(context)))
        {
        }

        public async Task<Account?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            return await _context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Username == username);
        }

        public async Task<bool> ExistsAsync(string username)
        {
            return await _context.Accounts.AnyAsync(a => a.Username == username);
        }

        public async Task<bool> ExistsEmailAsync(string email)
        {
            return await _context.Accounts.AnyAsync(a => a.Email == email);
        }

        public async Task AddAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task<Account?> GetByUsernameWithRolesAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            return await _context.Accounts
                .Include(a => a.AccountRoles.Where(ar => ar.IsActive))
                    .ThenInclude(ar => ar.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Username == username && a.IsActive);
        }

        public async Task<Account?> GetByIdWithRolesAsync(int accountId)
        {
            return await _context.Accounts
                .Include(a => a.AccountRoles.Where(ar => ar.IsActive))
                    .ThenInclude(ar => ar.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AccountId == accountId && a.IsActive);
        }

        public async Task<IEnumerable<Account>> GetStaffAccountsAsync()
        {
            return await _context.Accounts
                .Include(a => a.AccountRoles)
                    .ThenInclude(ar => ar.Role)
                .Where(a => a.IsActive &&
                            a.AccountRoles.Any(ar => ar.IsActive &&
                                ar.Role.RoleName != null &&
                                !ar.Role.RoleName.Equals("Customer", StringComparison.OrdinalIgnoreCase)))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Account>> GetCustomerAccountsAsync()
        {
            return await _context.Accounts
                .Include(a => a.AccountRoles)
                    .ThenInclude(ar => ar.Role)
                .Where(a => a.IsActive &&
                            a.AccountRoles.Any(ar => ar.IsActive &&
                                ar.Role.RoleName != null &&
                                ar.Role.RoleName.Equals("Customer", StringComparison.OrdinalIgnoreCase)))
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
