using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.DAL.EntityModels;
using EleVehicleDealer.DAL.Repositories.IRepository;
using EleVehicleDealer.DAL.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleVehicleDealer.BLL.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        public VehicleService(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        }

        public async Task<IEnumerable<EvdmsVehicle>> GetAllAsync()
            => await _vehicleRepository.GetAllAsync();

        public async Task<EvdmsVehicle> GetByIdAsync(int id)
            => await _vehicleRepository.GetByIdAsync(id);

        public async Task<EvdmsVehicle> CreateAsync(EvdmsVehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            return await _vehicleRepository.CreateVehicleAsync(vehicle);
        }

        public async Task<EvdmsVehicle> UpdateAsync(EvdmsVehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            // Ensure the vehicle exists
            var existingVehicle = await _vehicleRepository.GetByIdAsync(vehicle.VehicleId);
            if (existingVehicle == null)
                throw new ArgumentException($"Vehicle with ID {vehicle.VehicleId} not found");

            // Update the entity
            await _vehicleRepository.UpdateAsync(vehicle);
            return vehicle;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _vehicleRepository.DeleteByIdAsync(id);
        }

        public async Task<IEnumerable<EvdmsVehicle>> GetVehiclesByTypeAsync(string type)
            => await _vehicleRepository.GetVehiclesByTypeAsync(type);

        public async Task<IEnumerable<EvdmsVehicle>> GetAvailableVehiclesAsync()
            => await _vehicleRepository.GetAvailableVehiclesAsync();

        public async Task<IEnumerable<EvdmsVehicle>> SearchVehiclesAsync(string searchTerm)
            => await _vehicleRepository.SearchVehiclesAsync(searchTerm);

        public async Task<bool> UpdateStockQuantityAsync(int vehicleId, int quantity)
            => await _vehicleRepository.UpdateStockQuantityAsync(vehicleId, quantity);

        public async Task<IEnumerable<EvdmsVehicle>> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
            => await _vehicleRepository.GetVehiclesByPriceRangeAsync(minPrice, maxPrice);

        public async Task<int> GetTotalStockAsync()
            => await _vehicleRepository.GetTotalStockAsync();

        public async Task<IEnumerable<EvdmsOrder>> GetVehicleOrderHistoryAsync(int vehicleId)
            => await _vehicleRepository.GetVehicleOrderHistoryAsync(vehicleId);
    }
}
