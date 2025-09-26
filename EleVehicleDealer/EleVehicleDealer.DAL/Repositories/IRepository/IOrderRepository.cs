﻿using EleVehicleDealer.DAL.EntityModels;
using EleVehicleDealer.DAL.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleVehicleDealer.DAL.Repositories.IRepository
{
    public interface IOrderRepository : IGenericRepository<Order>
    {

        Task<Order> CreateOrderAsync(Order order);

        Task<IEnumerable<Order>> GetAllOrdersAsync();

        //lấy order theo account id

        //lấy order theo stationId 

       
    }
}
