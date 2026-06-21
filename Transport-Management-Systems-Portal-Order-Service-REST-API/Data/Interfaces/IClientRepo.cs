using Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Pagination;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Models;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Data.Interfaces
{
    public interface IClientRepo
    {
        Task<bool> SaveChangesAsync();

        Task<bool> ShipperAccountExists(Guid uid);

        Task<Client?> GetClientById(Guid id);

        Task<Client?> GetClientByEmail(string email);

        Task<IEnumerable<Client>> GetAllClients();

        Task<PaginatedResult<Client>> GetAllClientsWithPagination(PaginationParameters parameters);

        Task CreateClient(Client client);

        Task UpdateClient(Client client);

        Task DeleteClient(Client client);

        Task UpsertClient(Client client);
    }
}