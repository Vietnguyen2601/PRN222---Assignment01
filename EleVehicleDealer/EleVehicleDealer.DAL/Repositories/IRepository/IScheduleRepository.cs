using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.DAL.Repositories.Base;

namespace EleVehicleDealer.DAL.Repositories.IRepository
{
    public interface IScheduleRepository : IGenericRepository<Schedule>
    {
        Task<Schedule> CreateScheduleAsync(Schedule schedule);
        Task<IEnumerable<Schedule>> GetAllSchedulesAsync();
        Task<IEnumerable<Schedule>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Schedule>> GetByStationAsync(int stationId);
        Task<IEnumerable<Schedule>> GetUpcomingSchedulesAsync(DateTime startDate);
        Task UpdateScheduleAsync(Schedule schedule);
        Task DeleteScheduleAsync(int id);
        Task UpdateStatusAsync(int id, string status);
    }
}
