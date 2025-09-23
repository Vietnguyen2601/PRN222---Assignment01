using EleVehicleDealer.DAL.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleVehicleDealer.BLL.Interfaces
{
    public interface IVehicleService
    {
        Task<IEnumerable<EvdmsVehicle>> GetAllAsync();
        Task<EvdmsVehicle> GetByIdAsync(int id);
        Task<EvdmsVehicle> CreateAsync(EvdmsVehicle vehicle);
        Task<EvdmsVehicle> UpdateAsync(EvdmsVehicle vehicle);
        Task<bool> DeleteAsync(int id);
    }
}
