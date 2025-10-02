using EleVehicleDealer.DAL.EntityModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleVehicleDealer.DAL.Repositories.IRepository
{
    public interface IStationCarRepository
    {
        Task GetStationCarByIdAsync(int stationCarId); 
        Task<IEnumerable> GetAllStationCarsAsync(); 
        Task CreateStationCarAsync(StationCar stationCar); 
        Task UpdateStationCarAsync(StationCar stationCar); 
        Task DeleteStationCarAsync(int stationCarId);
    }
}
