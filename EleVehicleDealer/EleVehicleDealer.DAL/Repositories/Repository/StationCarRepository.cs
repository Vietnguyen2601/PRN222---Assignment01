using EleVehicleDealer.DAL.DBContext;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.DAL.Repositories.Base;
using EleVehicleDealer.DAL.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace EleVehicleDealer.DAL.Repositories.Repository
{
    public class StationCarRepository : GenericRepository<StationCar>, IStationCarRepository
    {
        public StationCarRepository(EvdmsDatabaseContext context) : base(context ?? throw new ArgumentNullException(nameof(context)))
        {
        }

        public async Task<StationCar?> GetStationCarWithDetailsAsync(int stationCarId)
        {
            return await _context.StationCars
                .Include(sc => sc.Station)
                .Include(sc => sc.Vehicle)
                .AsNoTracking()
                .FirstOrDefaultAsync(sc => sc.StationCarId == stationCarId && sc.IsActive);
        }

        public async Task<IEnumerable<StationCar>> GetAllStationCarsAsync()
        {
            return await _context.StationCars
                .Include(sc => sc.Station)
                .Include(sc => sc.Vehicle)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<StationCar>> GetActiveInventoryAsync()
        {
            return await _context.StationCars
                .Include(sc => sc.Station)
                .Include(sc => sc.Vehicle)
                .Where(sc => sc.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<StationCar>> GetInventoryByStationAsync(int stationId)
        {
            return await _context.StationCars
                .Include(sc => sc.Vehicle)
                .Where(sc => sc.StationId == stationId && sc.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<StationCar>> GetInventoryByVehicleAsync(int vehicleId)
        {
            return await _context.StationCars
                .Include(sc => sc.Station)
                .Where(sc => sc.VehicleId == vehicleId && sc.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<StationCar> CreateStationCarAsync(StationCar stationCar)
        {
            if (stationCar == null)
                throw new ArgumentNullException(nameof(stationCar));

            await _context.StationCars.AddAsync(stationCar);
            await _context.SaveChangesAsync();
            return stationCar;
        }

        public async Task UpdateStationCarAsync(StationCar stationCar)
        {
            if (stationCar == null)
                throw new ArgumentNullException(nameof(stationCar));

            _context.StationCars.Update(stationCar);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStationCarAsync(int stationCarId)
        {
            var existing = await _context.StationCars.FirstOrDefaultAsync(sc => sc.StationCarId == stationCarId);
            if (existing == null)
                return;

            existing.IsActive = false;
            existing.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AdjustQuantityAsync(int stationCarId, int deltaQuantity)
        {
            var stationCar = await _context.StationCars.FirstOrDefaultAsync(sc => sc.StationCarId == stationCarId);
            if (stationCar == null)
                return false;

            var newQuantity = stationCar.Quantity + deltaQuantity;
            if (newQuantity < 0)
                throw new InvalidOperationException("Inventory quantity cannot be negative.");

            stationCar.Quantity = newQuantity;
            stationCar.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
