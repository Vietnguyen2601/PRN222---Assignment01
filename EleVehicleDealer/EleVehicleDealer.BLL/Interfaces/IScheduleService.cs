using System.Collections.Generic;
using System.Threading.Tasks;
using EleVehicleDealer.Domain.DTOs.Schedules;

namespace EleVehicleDealer.BLL.Interfaces
{
    public interface IScheduleService
    {
        Task<ScheduleDto?> CreateScheduleAsync(ScheduleCreateDto schedule);
        Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync();
        Task<ScheduleDto?> UpdateScheduleAsync(ScheduleUpdateDto schedule);
        Task DeleteScheduleAsync(int id);
        Task UpdateScheduleStatusAsync(int id, string status);
    }
}
