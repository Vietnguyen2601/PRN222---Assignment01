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
    public class VehicleRepository : GenericRepository<EvdmsVehicle>, IVehicleRepository
    {
        private readonly EvdmsDatabaseContext _context;

        public VehicleRepository(EvdmsDatabaseContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<EvdmsVehicle> CreateVehicleAsync(EvdmsVehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            if (string.IsNullOrWhiteSpace(vehicle.ModelName))
                throw new ArgumentException("Model name is required");

            if (vehicle.Price < 0)
                throw new ArgumentException("Price cannot be negative");

            if (vehicle.StockQuantity < 0)
                throw new ArgumentException("Stock quantity cannot be negative");

            try
            {
                vehicle.CreatedAt = DateTime.UtcNow;
                await _context.EvdmsVehicles.AddAsync(vehicle);
                await _context.SaveChangesAsync();
                return vehicle;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error creating vehicle in database", ex);
            }
        }

        public async Task<IEnumerable<EvdmsVehicle>> GetVehiclesByTypeAsync(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Vehicle type cannot be empty");

            return await _context.EvdmsVehicles
                .Where(v => v.Type == type)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<EvdmsVehicle>> GetAvailableVehiclesAsync()
        {
            return await _context.EvdmsVehicles
                .Where(v => v.StockQuantity > 0)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<EvdmsVehicle>> SearchVehiclesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await _context.EvdmsVehicles.AsNoTracking().ToListAsync();

            return await _context.EvdmsVehicles
                .Where(v => v.ModelName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           v.Type.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> UpdateStockQuantityAsync(int vehicleId, int quantity)
        {
            if (quantity < 0)
                throw new ArgumentException("Quantity cannot be negative");

            var vehicle = await _context.EvdmsVehicles.FindAsync(vehicleId);
            if (vehicle == null)
                return false;

            vehicle.StockQuantity = quantity;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EvdmsVehicle>> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            if (minPrice < 0 || maxPrice < 0)
                throw new ArgumentException("Price values cannot be negative");
            if (minPrice > maxPrice)
                throw new ArgumentException("Minimum price cannot exceed maximum price");

            return await _context.EvdmsVehicles
                .Where(v => v.Price >= minPrice && v.Price <= maxPrice)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> GetTotalStockAsync()
        {
            return await _context.EvdmsVehicles
                .AsNoTracking()
                .SumAsync(v => v.StockQuantity);
        }

        public async Task<IEnumerable<EvdmsOrder>> GetVehicleOrderHistoryAsync(int vehicleId)
        {
            return await _context.EvdmsOrders
                .Where(o => o.VehicleId == vehicleId)
                .OrderByDescending(o => o.OrderDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var vehicle = await _context.EvdmsVehicles.FindAsync(id);
            if (vehicle == null)
                return false;

            _context.EvdmsVehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
