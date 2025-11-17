using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.DAL.Repositories.IRepository;
using EleVehicleDealer.Domain.DTOs.Schedules;
using EleVehicleDealer.BLL.Mappers;

namespace EleVehicleDealer.BLL.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        public ScheduleService(IScheduleRepository scheduleRepository)
        {
            this._scheduleRepository = scheduleRepository;
        }

        public async Task<ScheduleDto?> CreateScheduleAsync(ScheduleCreateDto schedule)
        {
            var entity = new Schedule
            {
                CustomerId = schedule.CustomerId,
                StationCarId = schedule.StationCarId,
                ScheduleTime = schedule.ScheduleTime,
                Status = schedule.Status,
                CreatedAt = System.DateTime.Now,
                UpdatedAt = System.DateTime.Now,
                IsActive = true
            };

            var created = await _scheduleRepository.CreateScheduleAsync(entity);
            var all = await _scheduleRepository.GetAllSchedulesAsync();
            return all.FirstOrDefault(s => s.ScheduleId == created.ScheduleId).ToScheduleDto();
        }

        public async Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync()
        {
            var schedules = await _scheduleRepository.GetAllSchedulesAsync();
            return schedules.ToScheduleDtos();
        }

        public async Task<ScheduleDto?> UpdateScheduleAsync(ScheduleUpdateDto schedule)
        {
            var existing = await _scheduleRepository.GetByIdAsync(schedule.ScheduleId);
            if (existing == null) return null;

            existing.StationCarId = schedule.StationCarId;
            existing.CustomerId = schedule.CustomerId;
            existing.ScheduleTime = schedule.ScheduleTime;
            existing.Status = schedule.Status;
            existing.IsActive = schedule.IsActive;
            existing.UpdatedAt = System.DateTime.Now;

            await _scheduleRepository.UpdateScheduleAsync(existing);
            var all = await _scheduleRepository.GetAllSchedulesAsync();
            return all.FirstOrDefault(s => s.ScheduleId == schedule.ScheduleId).ToScheduleDto();
        }

        public async Task DeleteScheduleAsync(int id)
        {
            await _scheduleRepository.DeleteScheduleAsync(id);
        }

        public async Task UpdateScheduleStatusAsync(int id, string status)
        {
            await _scheduleRepository.UpdateStatusAsync(id, status);
        }
    }
}
