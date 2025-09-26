using EleVehicleDealer.BLL.Interfaces;
using EleVehicleDealer.DAL.DBContext;
using EleVehicleDealer.DAL.EntityModels;
using EleVehicleDealer.DAL.Repositories.IRepository;
using EleVehicleDealer.DAL.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleVehicleDealer.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly EvdmsDatabaseContext _context = new EvdmsDatabaseContext();

        public OrderService(IOrderRepository orderRepository, EvdmsDatabaseContext context)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            order.IsActive = true;
            return await _orderRepository.CreateOrderAsync(order);
        }

        //public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        //{
        //    return await _orderRepository.GetAllOrdersAsync();
        //}
    }
}
