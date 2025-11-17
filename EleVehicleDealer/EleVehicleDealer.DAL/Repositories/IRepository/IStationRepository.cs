using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EleVehicleDealer.Domain.EntityModels;

namespace EleVehicleDealer.DAL.Repositories.IRepository
{
    public interface IStationRepository
    {
        Task<IEnumerable<Station>> GetAllStationsAsync();
    }
}