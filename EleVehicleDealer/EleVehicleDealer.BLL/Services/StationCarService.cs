using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.DAL.DBContext;
using EleVehicleDealer.DAL.Repositories.IRepository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EleVehicleDealer.BLL.Services
{
    public class StationCarService : IStationCarService
    {
        private readonly EvdmsDatabaseContext _context;
        private readonly IStationCarRepository _stationCarRepository;
        private readonly ILogger<StationCarService> _logger;

        public StationCarService(EvdmsDatabaseContext context, IStationCarRepository stationCarRepository, ILogger<StationCarService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _stationCarRepository = stationCarRepository ?? throw new ArgumentNullException(nameof(stationCarRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int?> GetStationCarIdByStationNameAndModelAsync(string stationName, string model)
        {
            if (string.IsNullOrEmpty(stationName))
                throw new ArgumentException("Station name is required.", nameof(stationName));
            if (string.IsNullOrEmpty(model))
                throw new ArgumentException("Vehicle model is required.", nameof(model));

            try
            {
                _logger.LogInformation("Searching for StationCarId with StationName: {StationName}, Model: {Model}", stationName, model);

                // Tìm Station dựa trên StationName
                var station = await _context.Stations
                    .FirstOrDefaultAsync(s => s.StationName == stationName && s.IsActive);

                if (station == null)
                {
                    _logger.LogWarning("No active station found with StationName: {StationName}", stationName);
                    return null;
                }

                // Tìm Vehicle dựa trên Model
                var vehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.Model == model && v.IsActive);

                if (vehicle == null)
                {
                    _logger.LogWarning("No active vehicle found with Model: {Model}", model);
                    return null;
                }

                // Tìm StationCar dựa trên StationId và VehicleId
                var stationCar = await _context.StationCars
                    .FirstOrDefaultAsync(sc => sc.StationId == station.StationId &&
                                            sc.VehicleId == vehicle.VehicleId &&
                                            sc.IsActive);

                if (stationCar == null)
                {
                    _logger.LogWarning("No active StationCar found for StationId: {StationId}, VehicleId: {VehicleId}", station.StationId, vehicle.VehicleId);
                    return null;
                }

                _logger.LogInformation("Found StationCarId: {StationCarId} for StationName: {StationName}, Model: {Model}", stationCar.StationCarId, stationName, model);
                return stationCar.StationCarId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving StationCarId for StationName: {StationName}, Model: {Model}", stationName, model);
                throw;
            }
        }
    }
}
