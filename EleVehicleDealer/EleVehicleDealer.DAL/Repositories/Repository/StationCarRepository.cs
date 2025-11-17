using EleVehicleDealer.Domain.EntityModels;
using EleVehicleDealer.DAL.Repositories.Base;
using EleVehicleDealer.DAL.Repositories.IRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleVehicleDealer.DAL.Repositories.Repository
{
    public class StationCarRepository : GenericRepository<StationCar>, IStationCarRepository
    {
        public Task CreateStationCarAsync(StationCar stationCar)
        {
            throw new NotImplementedException();
        }

        public Task DeleteStationCarAsync(int stationCarId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable> GetAllStationCarsAsync()
        {
            throw new NotImplementedException();
        }

        public Task GetStationCarByIdAsync(int stationCarId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateStationCarAsync(StationCar stationCar)
        {
            throw new NotImplementedException();
        }
    }
}
