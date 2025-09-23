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
    public class VehicleRepository : BaseRepository<EvdmsVehicle>, IVehicleRepository
    {
        private readonly EvdmsDatabaseContext _context;

        public VehicleRepository(EvdmsDatabaseContext context, ILogger<BaseRepository<EvdmsVehicle>> logger
        ) : base(context, logger)
        {
            _context = context;
        }

        public async Task<EvdmsVehicle> CreateVehicleAsync(EvdmsVehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            // Validation
            if (string.IsNullOrWhiteSpace(vehicle.ModelName))
                throw new ArgumentException("Model name is required");

            if (vehicle.Price < 0)
                throw new ArgumentException("Price cannot be negative");

            if (vehicle.StockQuantity < 0)
                throw new ArgumentException("Stock quantity cannot be negative");

            try
            {
                vehicle.CreatedAt = DateTime.Now;

                // Add to context
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
            return await _context.EvdmsVehicles
                .Where(v => v.ModelName.Contains(searchTerm) || v.Type.Contains(searchTerm))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> UpdateStockQuantityAsync(int vehicleId, int quantity)
        {
            var vehicle = await _context.EvdmsVehicles.FindAsync(vehicleId);
            if (vehicle == null) return false;
            if (quantity < 0) throw new ArgumentException("Quantity cannot be negative");
            vehicle.StockQuantity = quantity;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EvdmsVehicle>> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.EvdmsVehicles
                .Where(v => v.Price >= minPrice && v.Price <= maxPrice)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> GetTotalStockAsync()
        {
            return await _context.EvdmsVehicles.SumAsync(v => v.StockQuantity);
        }

        public async Task<IEnumerable<EvdmsOrder>> GetVehicleOrderHistoryAsync(int vehicleId)
        {
            return await _context.EvdmsOrders
                .Where(o => o.VehicleId == vehicleId)
                .OrderByDescending(o => o.OrderDate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
