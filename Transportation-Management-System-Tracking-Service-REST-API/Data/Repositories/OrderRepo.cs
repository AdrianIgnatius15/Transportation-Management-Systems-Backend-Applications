using Microsoft.EntityFrameworkCore;
using Transportation_Management_System_Tracking_Service_REST_API.Data.Interfaces;
using Transportation_Management_System_Tracking_Service_REST_API.DTOs.Pagination;
using Transportation_Management_System_Tracking_Service_REST_API.Models;

namespace Transportation_Management_System_Tracking_Service_REST_API.Data.Repositories
{
    public class OrderRepo : IOrderRepo
    {
        private readonly TMSDbContext _dbContext;

        public OrderRepo(TMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateOrder(Order order)
        {
            await _dbContext.Orders.AddAsync(order);
        }

        public void DeleteOrder(Order order)
        {
            _dbContext.Orders.Remove(order);
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
            => await _dbContext.Orders.ToListAsync();

        public async Task<PaginatedResult<Order>> GetAllOrdersWithPagination(PaginationParameters parameters)
        {
            var orders = await _dbContext.Orders
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PaginatedResult<Order>(orders, orders.Count, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<PaginatedResult<Order>> GetAllOrdersByClientIdWithPagination(Guid clientId, PaginationOrderSearchParameters parameters)
        {
            var orders = await _dbContext.Orders
                    .Include(order => order.Client)
                    .Include(order => order.DeliveryAddress)
                    .Include(order => order.ShipmentAddress)
                .Where(order => order.ClientId == clientId)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .Distinct()
                .OrderBy(order => order.OrderNumber)
                .ToListAsync();

            return new PaginatedResult<Order>(orders, orders.Count, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<Order?> GetOrderById(Guid id)
            => await _dbContext.Orders
                .Include(order => order.Client)
                .Include(order => order.ShipmentAddress)
                .Include(order => order.DeliveryAddress)
                .Include(order => order.Shipments)
                    .ThenInclude(shipment => shipment.Pieces)
            .FirstOrDefaultAsync(order => order.Id == id);

        public async Task<bool> SaveChangesAsync()
            => await _dbContext.SaveChangesAsync() > 0;

        public void UpdateOrder(Order order)
        {
            _dbContext.Orders.Update(order);
        }

        public async Task<PaginatedResult<Order>> GetAllOrdersByClientEmailWithPagination(string email, PaginationOrderSearchParameters paginationParameters)
        {
            var orders = await _dbContext.Orders
                .Where(orders => orders.Client.ContactEmail.ToLower() == email)
                .Distinct()
                .OrderBy(order => order.ClientId)
                .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                .Take(paginationParameters.PageSize)
                .ToListAsync();

            return new PaginatedResult<Order>(orders, orders.Count, paginationParameters.PageNumber, paginationParameters.PageSize);
        }
    }
}