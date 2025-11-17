using EleVehicleDealer.Domain.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleVehicleDealer.BLL.Interfaces
{
    public interface IScheduleService
    {
        Task<Schedule> CreateScheduleAsync(Schedule schedule);
        Task<IEnumerable<Schedule>> GetAllSchedulesAsync();
        Task UpdateScheduleAsync(Schedule schedule);
        Task DeleteScheduleAsync(int id);
        Task UpdateScheduleStatusAsync(int id, string status);
    }
}
