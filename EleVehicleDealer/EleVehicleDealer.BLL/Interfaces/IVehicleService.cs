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
        Task<IEnumerable<Vehicle>> GetAllVehicleAsync();
        Task<Vehicle> GetByIdAsync(int id);
        Task<Vehicle> CreateAsync(Vehicle vehicle);
        Task<Vehicle> UpdateAsync(Vehicle vehicle);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Vehicle>> GetVehiclesByStationAsync(int stationId);
    }
}
