using EleVehicleDealer.DAL.EntityModels;
using EleVehicleDealer.DAL.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleVehicleDealer.DAL.Repositories.IRepository
{
    public interface IAccountRepository : IGenericRepository<EvdmsAccount>
    {
        Task<EvdmsAccount?> GetByUsernameAsync(string username);
    }
}
