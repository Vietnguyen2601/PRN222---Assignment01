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
                vehicle.Availability = true;
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
                .Where(v => v.Type == type && v.IsActive.GetValueOrDefault(false))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehicleAsync()
        {
            return await _context.Vehicles
                .Where(v => v.IsActive ?? false)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
        {
            return await _context.Vehicles
                .Where(v => v.IsActive ?? false)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> SearchVehiclesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await _context.Vehicles
                    .Where(v => v.IsActive.GetValueOrDefault(false))
                    .AsNoTracking()
                    .ToListAsync();

            return await _context.Vehicles
                .Where(v => (v.Model.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           v.Type.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) && v.IsActive.GetValueOrDefault(false))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> UpdateStockAvailabilityAsync(int vehicleId, int quantity)
        {
            if (quantity < 0)
                throw new ArgumentException("Quantity cannot be negative");

            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            if (vehicle == null || !vehicle.IsActive.GetValueOrDefault(false))
                return false;

            vehicle.Availability = quantity > 0;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            if (minPrice < 0 || maxPrice < 0)
                throw new ArgumentException("Price values cannot be negative");
            if (minPrice > maxPrice)
                throw new ArgumentException("Minimum price cannot exceed maximum price");

            return await _context.Vehicles
                .Where(v => v.Price >= minPrice && v.Price <= maxPrice && v.IsActive.GetValueOrDefault(false))
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
    }
}
