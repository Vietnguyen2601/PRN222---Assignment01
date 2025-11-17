using System.Collections.Generic;
using System.Threading.Tasks;
using EleVehicleDealer.Domain.DTOs.Vehicles;

namespace EleVehicleDealer.BLL.Interfaces
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleCatalogDto>> GetAllVehicleAsync();
        Task<VehicleDetailDto?> GetByIdAsync(int id);
        Task<VehicleDetailDto?> CreateAsync(VehicleCreateDto vehicle);
        Task<VehicleDetailDto?> UpdateAsync(VehicleUpdateDto vehicle);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<VehicleCatalogDto>> GetVehiclesByStationAsync(int stationId);
        Task<IEnumerable<VehicleCatalogDto>> GetVehiclesByTypeAsync(string type);
        Task<IEnumerable<VehicleCatalogDto>> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<VehicleCatalogDto>> SearchVehiclesAsync(string searchTerm);
        Task<IEnumerable<decimal>> GetVehiclePriceByModelAsync(string model);
    }
}
