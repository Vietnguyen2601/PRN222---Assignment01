using System.Collections.Generic;
using System.Threading.Tasks;
using EleVehicleDealer.Domain.DTOs.Stations;

namespace EleVehicleDealer.BLL.Interfaces
{
    public interface IStationService
    {
        Task<IEnumerable<StationSummaryDto>> GetAllStationsAsync();
        Task<IEnumerable<StationSummaryDto>> GetStationsByVehicleModelAsync(string vehicleModel);
    }
}
