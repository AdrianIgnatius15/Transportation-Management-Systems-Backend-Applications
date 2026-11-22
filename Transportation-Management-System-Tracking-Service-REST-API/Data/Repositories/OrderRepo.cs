using System;
using Microsoft.EntityFrameworkCore;
using Transportation_Management_System_Tracking_Service_REST_API.Data.Interfaces;
using Transportation_Management_System_Tracking_Service_REST_API.Models;

namespace Transportation_Management_System_Tracking_Service_REST_API.Data.Repositories;

public class OrderRepo : IOrderRepo
{
    private readonly TMSDbContext _dbContext;

    public OrderRepo(TMSDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Order?> GetOrderShipmentById(Guid id)
        => await _dbContext.Orders
        .FirstOrDefaultAsync(order => order.Id.Equals(id));
}
