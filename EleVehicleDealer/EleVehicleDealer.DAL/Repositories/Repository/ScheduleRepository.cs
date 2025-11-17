using EleVehicleDealer.DAL.DBContext;
using EleVehicleDealer.Domain.EntityModels;
using EleVehicleDealer.DAL.Repositories.Base;
using EleVehicleDealer.DAL.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleVehicleDealer.DAL.Repositories.Repository
{
    public class ScheduleRepository : GenericRepository<Schedule>, IScheduleRepository
    {

        private readonly EvdmsDatabaseContext _context;

        public ScheduleRepository(EvdmsDatabaseContext context) 
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
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

        public async Task<IEnumerable<Schedule>> GetAllAsync()
        {
            return await _context.Schedules.ToListAsync();
        }

        public async Task UpdateAsync(Schedule schedule)
        {
            _context.Schedules.Update(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var schedule = await GetByIdAsync(id);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            var schedule = await GetByIdAsync(id);
            if (schedule != null)
            {
                schedule.Status = status;
                schedule.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}
