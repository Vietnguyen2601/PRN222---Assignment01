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
        private readonly EvdmsDatabaseContext _context;

        public VehicleService(IVehicleRepository vehicleRepository, EvdmsDatabaseContext context)
        {
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Vehicle> CreateAsync(Vehicle vehicle)
        {
            vehicle.IsActive = true;
            return await _vehicleRepository.CreateVehicleAsync(vehicle);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _vehicleRepository.DeleteByIdAsync(id);
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehicleAsync()
        {
            return await _vehicleRepository.GetAllVehicleAsync();
        }

        public async Task<Vehicle> GetByIdAsync(int id)
        {
            return await _context.Vehicles.FindAsync(id); ;
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
            throw new NotImplementedException("UpdateStockQuantityAsync is not applicable without Availability. Please redefine the logic if needed.");
        }

        public async Task<Vehicle> UpdateAsync(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            Console.WriteLine($"Updating Vehicle: Id={vehicle.VehicleId}, Model={vehicle.Model}, Type={vehicle.Type}, Color={vehicle.Color}, Price={vehicle.Price}, IsActive={vehicle.IsActive}");
            if (vehicle.VehicleId <= 0)
                throw new ArgumentException("VehicleId is required and must be a positive integer.", nameof(vehicle.VehicleId));

            var existingVehicle = await _context.Vehicles.FindAsync(vehicle.VehicleId);
            if (existingVehicle == null)
            {
                Console.WriteLine($"Error: Vehicle with Id={vehicle.VehicleId} not found");
                return null;
            }

            _context.Entry(existingVehicle).CurrentValues.SetValues(vehicle);
            existingVehicle.UpdatedAt = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
                // Xác nhận từ DB
                var verifiedVehicle = await _context.Vehicles.AsNoTracking().FirstOrDefaultAsync(v => v.VehicleId == vehicle.VehicleId);
                Console.WriteLine($"Verified IsActive from DB after save: {verifiedVehicle?.IsActive}");
                Console.WriteLine($"Update successful for VehicleId={vehicle.VehicleId}, IsActive={existingVehicle.IsActive}");
                return existingVehicle;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.InnerException?.Message ?? ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesByStationAsync(int stationId)
        {
            return await _vehicleRepository.GetVehiclesByStationAsync(stationId);
        }

        public async Task<IEnumerable<decimal>> GetVehiclePriceByModelAsync(string model)
        {
            return await _vehicleRepository.GetVehiclePriceByModelAsync(model);
        }
    }
}
