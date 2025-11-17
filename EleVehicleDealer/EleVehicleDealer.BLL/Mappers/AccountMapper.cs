using System.Collections.Generic;
using System.Linq;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.Domain.DTOs.Accounts;

namespace EleVehicleDealer.BLL.Mappers
{
    public static class AccountMapper
    {
        public static AccountSummaryDto? ToAccountSummaryDto(this Account? account)
        {
            if (account == null) return null;

            return new AccountSummaryDto
            {
                AccountId = account.AccountId,
                Username = account.Username,
                Email = account.Email,
                ContactNumber = account.ContactNumber,
                IsActive = account.IsActive,
                Roles = account.AccountRoles
                    .Where(ar => ar.IsActive && ar.Role != null)
                    .Select(ar => ar.Role!.RoleName)
                    .ToList()
            };
        }

        public static AccountDetailDto? ToAccountDetailDto(this Account? account)
        {
            if (account == null) return null;

            return new AccountDetailDto
            {
                AccountId = account.AccountId,
                Username = account.Username,
                Email = account.Email,
                ContactNumber = account.ContactNumber,
                IsActive = account.IsActive,
                Roles = account.AccountRoles
                    .Where(ar => ar.IsActive && ar.Role != null)
                    .Select(ar => ar.Role!.RoleName)
                    .ToList(),
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt
            };
        }

        public static IEnumerable<AccountSummaryDto> ToAccountSummaryDtos(this IEnumerable<Account>? accounts) =>
            accounts?.Select(a => a.ToAccountSummaryDto()!).Where(dto => dto != null)! ?? Enumerable.Empty<AccountSummaryDto>();
    }
}
