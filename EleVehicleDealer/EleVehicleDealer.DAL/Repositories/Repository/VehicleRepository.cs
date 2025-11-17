using EleVehicleDealer.DAL.DBContext;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.DAL.Repositories.Base;
using EleVehicleDealer.DAL.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace EleVehicleDealer.DAL.Repositories.Repository
{
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(EvdmsDatabaseContext context) : base(context ?? throw new ArgumentNullException(nameof(context)))
        {
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

        public async Task<IEnumerable<Vehicle>> GetVehiclesByManufacturerAsync(string manufacturer)
        {
            if (string.IsNullOrWhiteSpace(manufacturer))
                throw new ArgumentException("Manufacturer cannot be empty");

            return await _context.Vehicles
                .Where(v => v.Manufacturer == manufacturer && v.IsActive)
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

        public async Task<IEnumerable<Vehicle>> GetActiveCatalogAsync()
        {
            return await _context.Vehicles
                .Where(v => v.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> SearchVehiclesAsync(string searchTerm)
        {
            IQueryable<Vehicle> query = _context.Vehicles.Where(v => v.IsActive);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(v =>
                    (v.Model != null && EF.Functions.Like(v.Model, $"%{searchTerm}%")) ||
                    (v.Type != null && EF.Functions.Like(v.Type, $"%{searchTerm}%")) ||
                    (v.Manufacturer != null && EF.Functions.Like(v.Manufacturer, $"%{searchTerm}%"))
                );
            }

            return await query.AsNoTracking().ToListAsync();
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

        public async Task<IEnumerable<Vehicle>> GetVehiclesByIdsAsync(IEnumerable<int> vehicleIds)
        {
            var ids = vehicleIds?.ToList() ?? new List<int>();
            if (!ids.Any())
                return Enumerable.Empty<Vehicle>();

            return await _context.Vehicles
                .Where(v => ids.Contains(v.VehicleId) && v.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.VehicleId == id);
            if (vehicle == null)
                return false;

            vehicle.IsActive = false;
            vehicle.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesByStationAsync(int stationId)
        {
            return await _context.Vehicles
                .Where(v => v.IsActive &&
                            v.StationCars.Any(sc => sc.StationId == stationId && sc.IsActive))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> GetTotalStockAsync()
        {
            return await _context.StationCars
                .Where(sc => sc.IsActive)
                .SumAsync(sc => sc.Quantity);
        }

        public async Task<IEnumerable<Order>> GetVehicleOrderHistoryAsync(int vehicleId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.IsActive &&
                            o.OrderItems.Any(oi => oi.StationCar != null && oi.StationCar.VehicleId == vehicleId))
                .OrderByDescending(o => o.OrderDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<decimal>> GetVehiclePriceByModelAsync(string model)
        {
            if (string.IsNullOrWhiteSpace(model))
                throw new ArgumentException("Model cannot be null or empty.", nameof(model));

            return await _context.Vehicles
                .Where(v => v.Model == model && v.IsActive)
                .AsNoTracking()
                .Select(v => v.Price)
                .ToListAsync();
        }
    }
}
