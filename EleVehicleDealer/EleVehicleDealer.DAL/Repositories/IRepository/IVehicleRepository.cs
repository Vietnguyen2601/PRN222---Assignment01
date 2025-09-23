using EleVehicleDealer.DAL.EntityModels;
using EleVehicleDealer.DAL.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleVehicleDealer.DAL.Repositories.IRepository
{
    public interface IVehicleRepository : IBaseRepository<EvdmsVehicle>
    {
        Task<EvdmsVehicle> CreateVehicleAsync(EvdmsVehicle vehicle);
        Task<IEnumerable<EvdmsVehicle>> GetVehiclesByTypeAsync(string type);
        Task<IEnumerable<EvdmsVehicle>> GetAvailableVehiclesAsync();
        Task<IEnumerable<EvdmsVehicle>> SearchVehiclesAsync(string searchTerm);
        Task<bool> UpdateStockQuantityAsync(int vehicleId, int quantity);
        Task<IEnumerable<EvdmsVehicle>> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<int> GetTotalStockAsync();
        Task<IEnumerable<EvdmsOrder>> GetVehicleOrderHistoryAsync(int vehicleId);
    }
}
