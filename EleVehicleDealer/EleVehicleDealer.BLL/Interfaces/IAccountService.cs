using EleVehicleDealer.DAL.EntityModels;
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
    }
}
