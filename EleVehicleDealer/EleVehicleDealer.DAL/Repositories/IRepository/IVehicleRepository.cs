using EleVehicleDealer.Domain.EntityModels;
using EleVehicleDealer.DAL.Repositories.Base;


namespace EleVehicleDealer.DAL.Repositories.IRepository
{
    public interface IVehicleRepository : IGenericRepository<Vehicle>
    {
        Task<Vehicle> CreateVehicleAsync(Vehicle vehicle);
        Task<IEnumerable<Vehicle>> GetAllVehicleAsync();
        Task<IEnumerable<Vehicle>> GetVehiclesByTypeAsync(string type);
        Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync();
        Task<IEnumerable<Vehicle>> SearchVehiclesAsync(string searchTerm);
        Task<bool> UpdateStockAvailabilityAsync(int vehicleId, int quantity);
        Task<IEnumerable<Vehicle>> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<bool> DeleteByIdAsync(int id);
        Task<IEnumerable<Vehicle>> GetVehiclesByStationAsync(int stationId);
        Task<int> GetTotalStockAsync();
        Task<IEnumerable<Order>> GetVehicleOrderHistoryAsync(int vehicleId);
        Task<IEnumerable<decimal>> GetVehiclePriceByModelAsync(string model);
    }
}
