using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.DAL.DBContext;
using EleVehicleDealer.Domain.DTOs.Dashboard;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EleVehicleDealer.BLL.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly EvdmsDatabaseContext _context;

        public DashboardService(EvdmsDatabaseContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> GetDashboardDataAsync()
        {
            var currentYear = DateTime.Now.Year;

            var dashboard = new DashboardDto
            {
                TotalRevenue = await _context.Orders
                    .Where(o => o.IsActive)
                    .SelectMany(o => o.OrderItems)
                    .SumAsync(oi => oi.Price * oi.Quantity),

                TotalOrders = await _context.Orders
                    .Where(o => o.IsActive)
                    .CountAsync(),

                TotalCustomers = await _context.Accounts
                    .Where(a => a.IsActive)
                    .CountAsync(),

                TotalVehicles = await _context.Vehicles
                    .Where(v => v.IsActive)
                    .CountAsync(),

                RevenueByQuarter = await GetRevenueByQuarterAsync(currentYear),
                RevenueByMonth = await GetRevenueByMonthAsync(currentYear),
                RevenueByStation = await GetRevenueByStationAsync()
            };

            return dashboard;
        }

        public async Task<List<RevenueByPeriodDto>> GetRevenueByQuarterAsync(int year)
        {
            var orders = await _context.Orders
                .Where(o => o.IsActive && o.OrderDate.Year == year)
                .Include(o => o.OrderItems)
                .ToListAsync();

            var revenueByQuarter = orders
                .GroupBy(o => (o.OrderDate.Month - 1) / 3 + 1)
                .Select(g => new RevenueByPeriodDto
                {
                    Period = $"Q{g.Key} {year}",
                    Revenue = g.SelectMany(o => o.OrderItems).Sum(oi => oi.Price * oi.Quantity),
                    OrderCount = g.Count()
                })
                .OrderBy(r => r.Period)
                .ToList();

            // Đảm bảo có đủ 4 quý
            for (int i = 1; i <= 4; i++)
            {
                if (!revenueByQuarter.Any(r => r.Period == $"Q{i} {year}"))
                {
                    revenueByQuarter.Add(new RevenueByPeriodDto
                    {
                        Period = $"Q{i} {year}",
                        Revenue = 0,
                        OrderCount = 0
                    });
                }
            }

            return revenueByQuarter.OrderBy(r => r.Period).ToList();
        }

        public async Task<List<RevenueByPeriodDto>> GetRevenueByMonthAsync(int year)
        {
            var orders = await _context.Orders
                .Where(o => o.IsActive && o.OrderDate.Year == year)
                .Include(o => o.OrderItems)
                .ToListAsync();

            var revenueByMonth = orders
                .GroupBy(o => o.OrderDate.Month)
                .Select(g => new RevenueByPeriodDto
                {
                    Period = $"{g.Key:D2}/{year}",
                    Revenue = g.SelectMany(o => o.OrderItems).Sum(oi => oi.Price * oi.Quantity),
                    OrderCount = g.Count()
                })
                .ToList();

            // Đảm bảo có đủ 12 tháng
            for (int i = 1; i <= 12; i++)
            {
                if (!revenueByMonth.Any(r => r.Period == $"{i:D2}/{year}"))
                {
                    revenueByMonth.Add(new RevenueByPeriodDto
                    {
                        Period = $"{i:D2}/{year}",
                        Revenue = 0,
                        OrderCount = 0
                    });
                }
            }

            return revenueByMonth.OrderBy(r => r.Period).ToList();
        }

        public async Task<CustomDateRangeDto> GetRevenueByCustomDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _context.Orders
                .Where(o => o.IsActive && o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Include(o => o.OrderItems)
                .ToListAsync();

            var dailyRevenue = orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new RevenueByPeriodDto
                {
                    Period = g.Key.ToString("dd/MM/yyyy"),
                    Revenue = g.SelectMany(o => o.OrderItems).Sum(oi => oi.Price * oi.Quantity),
                    OrderCount = g.Count()
                })
                .OrderBy(r => r.Period)
                .ToList();

            return new CustomDateRangeDto
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalRevenue = dailyRevenue.Sum(d => d.Revenue),
                TotalOrders = dailyRevenue.Sum(d => d.OrderCount),
                DailyRevenue = dailyRevenue
            };
        }

        public async Task<List<StationRevenueDto>> GetRevenueByStationAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Orders
                .Where(o => o.IsActive)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.StationCar)
                        .ThenInclude(sc => sc.Station)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= endDate.Value);
            }

            var orders = await query.ToListAsync();

            var revenueByStation = orders
                .SelectMany(o => o.OrderItems.Select(oi => new
                {
                    StationName = oi.StationCar?.Station?.StationName ?? "Unknown",
                    Revenue = oi.Price * oi.Quantity,
                    Quantity = oi.Quantity
                }))
                .GroupBy(x => x.StationName)
                .Select(g => new StationRevenueDto
                {
                    StationName = g.Key,
                    Revenue = g.Sum(x => x.Revenue),
                    OrderCount = g.Count(),
                    VehiclesSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(s => s.Revenue)
                .ToList();

            return revenueByStation;
        }
    }
}
