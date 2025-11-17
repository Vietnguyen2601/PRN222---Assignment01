using System;

namespace EleVehicleDealer.Domain.DTOs.Schedules
{
    public class ScheduleDto
    {
        public int ScheduleId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int StationCarId { get; set; }
        public string VehicleModel { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public DateTime ScheduleTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class ScheduleCreateDto
    {
        public int CustomerId { get; set; }
        public int StationCarId { get; set; }
        public DateTime ScheduleTime { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class ScheduleUpdateDto : ScheduleCreateDto
    {
        public int ScheduleId { get; set; }
        public bool IsActive { get; set; }
    }
}
