using System.Collections.Generic;
using System.Threading.Tasks;
using EleVehicleDealer.DAL.Models;

namespace EleVehicleDealer.DAL.Repositories.IRepository
{
    public interface IStationRepository
    {
        Task<IEnumerable<Station>> GetAllStationsAsync();
        Task<Station?> GetByIdAsync(int stationId);
        Task<IEnumerable<Station>> GetActiveStationsAsync();
    }
}
