using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.DAL.DBContext;
using EleVehicleDealer.DAL.EntityModels;
using EleVehicleDealer.DAL.Repositories.IRepository;
using EleVehicleDealer.DAL.Repositories.Repository;
using Microsoft.EntityFrameworkCore;
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
        private readonly EvdmsDatabaseContext _context = new EvdmsDatabaseContext();

        public VehicleService(IVehicleRepository vehicleRepository, EvdmsDatabaseContext context)
        {
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Vehicle> CreateAsync(Vehicle vehicle)
        {
            return await _vehicleRepository.CreateVehicleAsync(vehicle);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _vehicleRepository.DeleteByIdAsync(id);
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            var vehicles = await _vehicleRepository.GetAllAsync();
            return vehicles.Where(v => v.IsActive.GetValueOrDefault(false));
        }

        public async Task<Vehicle> GetByIdAsync(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null || !vehicle.IsActive.GetValueOrDefault(false))
                return null;
            return vehicle;
        }

        public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
        {
            return await _vehicleRepository.GetAvailableVehiclesAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _vehicleRepository.GetVehiclesByPriceRangeAsync(minPrice, maxPrice);
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesByTypeAsync(string type)
        {
            return await _vehicleRepository.GetVehiclesByTypeAsync(type);
        }

        public async Task<IEnumerable<Order>> GetVehicleOrderHistoryAsync(int vehicleId)
        {
            return await _vehicleRepository.GetVehicleOrderHistoryAsync(vehicleId);
        }

        public async Task<int> GetTotalStockAsync()
        {
            return await _vehicleRepository.GetTotalStockAsync();
        }

        public async Task<IEnumerable<Vehicle>> SearchVehiclesAsync(string searchTerm)
        {
            return await _vehicleRepository.SearchVehiclesAsync(searchTerm);
        }

        public async Task<bool> UpdateStockQuantityAsync(int vehicleId, int quantity)
        {
            return await _vehicleRepository.UpdateStockAvailabilityAsync(vehicleId, quantity);
        }

        public async Task<Vehicle> UpdateAsync(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            var existingVehicle = await _context.Vehicles.FindAsync(vehicle.VehicleId);
            if (existingVehicle == null || !existingVehicle.IsActive.GetValueOrDefault(false))
                return null;

            existingVehicle.Model = vehicle.Model;
            existingVehicle.Type = vehicle.Type;
            existingVehicle.Color = vehicle.Color;
            existingVehicle.Price = vehicle.Price;
            existingVehicle.Availability = vehicle.Availability;
            existingVehicle.StationId = vehicle.StationId;

            await _context.SaveChangesAsync();
            return existingVehicle;
        }
    }
}
