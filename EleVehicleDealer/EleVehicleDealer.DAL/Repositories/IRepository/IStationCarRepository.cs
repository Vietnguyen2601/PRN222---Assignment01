using System.Collections.Generic;
using System.Threading.Tasks;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.DAL.Repositories.Base;

namespace EleVehicleDealer.DAL.Repositories.IRepository
{
    public interface IStationCarRepository : IGenericRepository<StationCar>
    {
        Task<StationCar?> GetStationCarWithDetailsAsync(int stationCarId);
        Task<IEnumerable<StationCar>> GetAllStationCarsAsync();
        Task<IEnumerable<StationCar>> GetActiveInventoryAsync();
        Task<IEnumerable<StationCar>> GetInventoryByStationAsync(int stationId);
        Task<IEnumerable<StationCar>> GetInventoryByVehicleAsync(int vehicleId);
        Task<StationCar> CreateStationCarAsync(StationCar stationCar);
        Task UpdateStationCarAsync(StationCar stationCar);
        Task DeleteStationCarAsync(int stationCarId);
        Task<bool> AdjustQuantityAsync(int stationCarId, int deltaQuantity);
    }
}
