using EleVehicleDealer.DAL.DBContext;
using EleVehicleDealer.DAL.EntityModels;
using EleVehicleDealer.DAL.Repositories.Base;
using EleVehicleDealer.DAL.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleVehicleDealer.DAL.Repositories.Repository
{
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {
        private readonly EvdmsDatabaseContext _context;

        public VehicleRepository(EvdmsDatabaseContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Vehicle> CreateVehicleAsync(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            if (string.IsNullOrWhiteSpace(vehicle.Model))
                throw new ArgumentException("Model name is required");

            if (vehicle.Price < 0)
                throw new ArgumentException("Price cannot be negative");

            try
            {
                vehicle.IsActive = true;
                await _context.Vehicles.AddAsync(vehicle);
                await _context.SaveChangesAsync();
                return vehicle;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error creating vehicle in database", ex);
            }
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesByTypeAsync(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Vehicle type cannot be empty");

            return await _context.Vehicles
                .Where(v => v.Type == type && v.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehicleAsync()
        {
            return await _context.Vehicles
                .Include(v => v.StationCars)
                .ThenInclude(sc => sc.Station)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
        {
            return await _context.Vehicles
                .Where(v => v.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> SearchVehiclesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await _context.Vehicles
                    .Where(v => v.IsActive)
                    .AsNoTracking()
                    .ToListAsync();

            return await _context.Vehicles
                .Where(v => (v.Model.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           v.Type.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) && v.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> UpdateStockAvailabilityAsync(int vehicleId, int quantity)
        {
            throw new NotImplementedException("UpdateStockAvailabilityAsync is not applicable without Availability. Please redefine the logic if needed.");
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            if (minPrice < 0 || maxPrice < 0)
                throw new ArgumentException("Price values cannot be negative");
            if (minPrice > maxPrice)
                throw new ArgumentException("Minimum price cannot exceed maximum price");

            return await _context.Vehicles
                .Where(v => v.Price >= minPrice && v.Price <= maxPrice && v.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
                return false;

            vehicle.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesByStationAsync(int stationId)
        {
            return await _context.Vehicles
                .Where(v => v.StationCars.Any(sc => sc.StationId == stationId) && v.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> GetTotalStockAsync()
        {
            return await _context.Vehicles
                .Where(v => v.IsActive)
                .CountAsync();
        }

        public async Task<IEnumerable<Order>> GetVehicleOrderHistoryAsync(int vehicleId)
        {
            return await _context.Orders
                .Where(o => o.IsActive) 
                .OrderByDescending(o => o.OrderDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<decimal>> GetVehiclePriceByModelAsync(string model)
        {
            if (string.IsNullOrWhiteSpace(model))
            {
                throw new ArgumentException("Model cannot be null or empty.", nameof(model));
            }

            return await _context.Vehicles
                .Where(v => v.Model == model && v.IsActive)
                .AsNoTracking()
                .Select(v => v.Price)
                .ToListAsync();
        }
    }
}
