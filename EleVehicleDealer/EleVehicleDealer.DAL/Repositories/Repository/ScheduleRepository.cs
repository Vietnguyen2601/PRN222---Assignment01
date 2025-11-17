using System;
using System.Collections.Generic;
using System.Linq;
using EleVehicleDealer.DAL.DBContext;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.DAL.Repositories.Base;
using EleVehicleDealer.DAL.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace EleVehicleDealer.DAL.Repositories.Repository
{
    public class ScheduleRepository : GenericRepository<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(EvdmsDatabaseContext context) : base(context ?? throw new ArgumentNullException(nameof(context)))
        {
        }
        public async Task<Schedule> CreateScheduleAsync(Schedule schedule)
        {
            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule), "Schedule cannot be null.");

            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(schedule.Status))
                    throw new ArgumentException("Schedule status cannot be null or empty.");

                // Add the schedule to the database
                await _context.Schedules.AddAsync(schedule);
                await _context.SaveChangesAsync();
                return schedule;
            }
            catch (DbUpdateException ex)
            {
                // Log the exception details
                Console.WriteLine($"DbUpdateException: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                // Re-throw with additional context
                throw new Exception("Error creating schedule in database. Ensure all required fields are valid.", ex);
            }
            catch (Exception ex)
            {
                // Catch other exceptions
                throw new Exception("An unexpected error occurred while creating the schedule.", ex);
            }
        }

        public async Task<IEnumerable<Schedule>> GetAllSchedulesAsync()
        {
            return await _context.Schedules
                .Include(s => s.Customer)
                .Include(s => s.StationCar)
                    .ThenInclude(sc => sc.Vehicle)
                .Where(s => s.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Schedule>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Schedules
                .Where(s => s.CustomerId == customerId && s.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Schedule>> GetByStationAsync(int stationId)
        {
            return await _context.Schedules
                .Where(s => s.IsActive &&
                            s.StationCar != null &&
                            s.StationCar.StationId == stationId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Schedule>> GetUpcomingSchedulesAsync(DateTime startDate)
        {
            return await _context.Schedules
                .Where(s => s.IsActive && s.ScheduleTime >= startDate)
                .OrderBy(s => s.ScheduleTime)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateScheduleAsync(Schedule schedule)
        {
            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule));

            _context.Schedules.Update(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteScheduleAsync(int id)
        {
            var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.ScheduleId == id);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.ScheduleId == id);
            if (schedule != null)
            {
                schedule.Status = status;
                schedule.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}
