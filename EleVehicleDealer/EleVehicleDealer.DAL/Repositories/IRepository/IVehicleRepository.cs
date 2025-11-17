using System.Collections.Generic;
using System.Threading.Tasks;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.DAL.Repositories.Base;

namespace EleVehicleDealer.DAL.Repositories.IRepository
{
    public interface IVehicleRepository : IGenericRepository<Vehicle>
    {
        Task<Vehicle> CreateVehicleAsync(Vehicle vehicle);
        Task<IEnumerable<Vehicle>> GetAllVehicleAsync();
        Task<IEnumerable<Vehicle>> GetActiveCatalogAsync();
        Task<IEnumerable<Vehicle>> GetVehiclesByTypeAsync(string type);
        Task<IEnumerable<Vehicle>> GetVehiclesByManufacturerAsync(string manufacturer);
        Task<IEnumerable<Vehicle>> SearchVehiclesAsync(string searchTerm);
        Task<IEnumerable<Vehicle>> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<Vehicle>> GetVehiclesByIdsAsync(IEnumerable<int> vehicleIds);
        Task<bool> DeleteByIdAsync(int id);
        Task<IEnumerable<Vehicle>> GetVehiclesByStationAsync(int stationId);
        Task<int> GetTotalStockAsync();
        Task<IEnumerable<Order>> GetVehicleOrderHistoryAsync(int vehicleId);
        Task<IEnumerable<decimal>> GetVehiclePriceByModelAsync(string model);
    }
}
