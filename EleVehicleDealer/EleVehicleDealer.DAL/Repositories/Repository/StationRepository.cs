using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EleVehicleDealer.DAL.DBContext;
using EleVehicleDealer.Domain.EntityModels;
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
            .Where(s => s.IsActive == true)
            .AsNoTracking()
            .ToListAsync();
        }
    }
}