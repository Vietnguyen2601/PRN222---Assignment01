using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EleVehicleDealer.DAL.DBContext;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.DAL.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace EleVehicleDealer.DAL.Repositories.Repository
{
    public class StationRepository : IStationRepository
    {
        private readonly EvdmsDatabaseContext _context;
        public StationRepository(EvdmsDatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<Station>> GetAllStationsAsync()
        {
            return await _context.Stations
                .Include(s => s.StationCars)
                    .ThenInclude(sc => sc.Vehicle)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Station>> GetActiveStationsAsync()
        {
            return await _context.Stations
                .Where(s => s.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Station?> GetByIdAsync(int stationId)
        {
            return await _context.Stations
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.StationId == stationId);
        }
    }
}
