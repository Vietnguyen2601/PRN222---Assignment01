using System.Collections.Generic;
using System.Linq;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.Domain.DTOs.Schedules;

namespace EleVehicleDealer.BLL.Mappers
{
    public static class ScheduleMapper
    {
        public static ScheduleDto? ToScheduleDto(this Schedule? schedule)
        {
            if (schedule == null) return null;

            return new ScheduleDto
            {
                ScheduleId = schedule.ScheduleId,
                CustomerId = schedule.CustomerId,
                CustomerName = schedule.Customer?.Username ?? string.Empty,
                StationCarId = schedule.StationCarId,
                VehicleModel = schedule.StationCar?.Vehicle?.Model ?? string.Empty,
                StationName = schedule.StationCar?.Station?.StationName ?? string.Empty,
                ScheduleTime = schedule.ScheduleTime,
                Status = schedule.Status,
                IsActive = schedule.IsActive
            };
        }

        public static IEnumerable<ScheduleDto> ToScheduleDtos(this IEnumerable<Schedule>? schedules) =>
            schedules?.Select(s => s.ToScheduleDto()!).Where(dto => dto != null)! ?? Enumerable.Empty<ScheduleDto>();
    }
}
