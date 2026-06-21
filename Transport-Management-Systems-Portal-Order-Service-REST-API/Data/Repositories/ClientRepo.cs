using Microsoft.EntityFrameworkCore;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Data.Interfaces;
using Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Pagination;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Models;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Data.Repositories
{
    public class ClientRepo : IClientRepo
    {
        private readonly TMSDbContext _dbContext;

        public ClientRepo(TMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateClient(Client client)
            => await _dbContext.Clients.AddAsync(client);

        public async Task DeleteClient(Client client)
            => _dbContext.Clients.Remove(client);

        public async Task<IEnumerable<Client>> GetAllClients()
            => await _dbContext.Clients.ToListAsync();

        public async Task<Client?> GetClientById(Guid id)
            => await _dbContext.Clients.FirstOrDefaultAsync(client => client.Id.Equals(id));

        public async Task UpdateClient(Client client)
            => _dbContext.Clients.Update(client);

        public async Task UpsertClient(Client client)
        {
            var clientFound = await _dbContext.Clients.FirstOrDefaultAsync(clientProfile => clientProfile.ContactEmail == client.ContactEmail);
            if (clientFound == null)
            {
                await _dbContext.Clients.AddAsync(client);
            }
            else
            {
                _dbContext.Clients.Update(clientFound);
            }
        }

        public async Task<bool> SaveChangesAsync()
            => await _dbContext.SaveChangesAsync() > 0;

        public async Task<PaginatedResult<Client>> GetAllClientsWithPagination(PaginationParameters parameters)
        {
            var clients = await _dbContext.Clients
                    .Skip((parameters.PageNumber - 1) * parameters.PageNumber)
                    .Take(parameters.PageSize)
                    .ToListAsync();

            return new PaginatedResult<Client>(clients, clients.Count, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<Client?> GetClientByEmail(string email)
         => await _dbContext.Clients.FirstOrDefaultAsync(client => client.ContactEmail == email);

        public async Task<bool> ShipperAccountExists(Guid uid)
            => _dbContext.Clients.Any(client => client.Id == uid);
    }
}