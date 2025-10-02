using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleVehicleDealer.BLL.Interfaces
{
    public interface IStationCardService
    {
        Task<int?> GetStationCarIdByStationNameAndModelAsync(string stationName, string model);
    }
}
