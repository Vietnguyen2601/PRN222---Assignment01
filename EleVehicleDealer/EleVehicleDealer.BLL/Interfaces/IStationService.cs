using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EleVehicleDealer.Domain.EntityModels;

namespace EleVehicleDealer.BLL.Interfaces
{
    public interface IStationService
    {
        Task<IEnumerable<Station>> GetAllStationsAsync();
    }
}