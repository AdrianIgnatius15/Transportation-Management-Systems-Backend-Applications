using Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Pagination;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Models;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Data.Interfaces
{
    public interface IOrderRepo
    {
        Task<bool> SaveChangesAsync();

        Task<IEnumerable<Order>> GetAllOrders();

        Task<PaginatedResult<Order>> GetAllOrdersWithPagination(PaginationParameters parameters);

        Task<PaginatedResult<Order>> GetAllOrdersByClientIdWithPagination(Guid clientId, PaginationOrderSearchParameters parameters);

        Task<PaginatedResult<Order>> GetAllOrdersByClientEmailWithPagination(string email, PaginationOrderSearchParameters paginationParameters);

        Task<Order?> GetOrderById(Guid id);

        Task CreateOrder(Order order);

        void UpdateOrder(Order order);

        void DeleteOrder(Order order);
    }
}