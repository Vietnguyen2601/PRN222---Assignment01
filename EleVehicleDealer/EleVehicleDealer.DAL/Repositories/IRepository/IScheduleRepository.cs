using EleVehicleDealer.Domain.EntityModels;
using EleVehicleDealer.DAL.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleVehicleDealer.DAL.Repositories.IRepository
{
    public interface IScheduleRepository : IGenericRepository<Schedule>
    {
        Task<Schedule> CreateScheduleAsync(Schedule schedule);
        Task<IEnumerable<Schedule>> GetAllAsync();
        Task UpdateAsync(Schedule schedule);
        Task DeleteAsync(int id);
        Task UpdateStatusAsync(int id, string status);
    }
}
