using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.DAL.Repositories.IRepository;
using EleVehicleDealer.Domain.DTOs.Stations;
using EleVehicleDealer.BLL.Mappers;

namespace EleVehicleDealer.BLL.Services
{
    public class StationService : IStationService
    {
        private readonly IStationRepository _stationRepository;
        public StationService(IStationRepository stationRepository)
        {
            _stationRepository = stationRepository;
        }
        public async Task<IEnumerable<StationSummaryDto>> GetAllStationsAsync()
        {
            var stations = await _stationRepository.GetAllStationsAsync();
            return stations.ToStationSummaryDtos();
        }

        public async Task<IEnumerable<StationSummaryDto>> GetStationsByVehicleModelAsync(string vehicleModel)
        {
            var allStations = await _stationRepository.GetAllStationsAsync();
            var stationsWithVehicle = allStations
                .Where(s => s.IsActive && 
                       s.StationCars.Any(sc => sc.IsActive && 
                                              sc.Vehicle != null && 
                                              sc.Vehicle.Model == vehicleModel &&
                                              sc.Quantity > 0))
                .ToList();
            return stationsWithVehicle.ToStationSummaryDtos();
        }
    }
}
