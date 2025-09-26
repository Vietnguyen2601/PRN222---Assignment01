using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.DAL.EntityModels;
using EleVehicleDealer.DAL.Repositories.IRepository;

namespace EleVehicleDealer.BLL.Services
{
    public class StationService : IStationService
    {
        private readonly IStationRepository _stationRepository;
        public StationService(IStationRepository stationRepository)
        {
            _stationRepository = stationRepository;
        }
        public async Task<IEnumerable<Station>> GetAllStationsAsync()
        {
            return await _stationRepository.GetAllStationsAsync();
        }
    }
}