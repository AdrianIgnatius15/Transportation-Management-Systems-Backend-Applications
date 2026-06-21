using Microsoft.EntityFrameworkCore;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Data.Interfaces;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Models;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Data.Repositories
{
    public class AddressRepo : IAddressRepo
    {
        private readonly TMSDbContext _dbContext;

        public AddressRepo(TMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateNewAddressAsync(Address address)
        {
            await _dbContext.Addresses.AddAsync(address);
        }

        public async Task DeleteAddressAsync(Address address)
        {
            _dbContext.Addresses.Remove(address);
        }

        public async Task<IEnumerable<Address>> GetAddressesAsync()
            => await _dbContext.Addresses.ToListAsync();

        public async Task UpdateAddressAsync(Address updatedAddress)
        {
            _dbContext.Addresses.Update(updatedAddress);
        }

        public async Task<bool> SaveChangesAsync()
            => await _dbContext.SaveChangesAsync() > 0;
    }
}