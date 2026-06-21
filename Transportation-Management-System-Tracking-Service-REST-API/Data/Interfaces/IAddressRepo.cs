using Transportation_Management_System_Tracking_Service_REST_API.Models;

namespace Transportation_Management_System_Tracking_Service_REST_API.Data.Interfaces
{
    public interface IAddressRepo
    {
        Task<bool> SaveChangesAsync();
        Task<IEnumerable<Address>> GetAddressesAsync();

        Task CreateNewAddressAsync(Address address);

        Task UpdateAddressAsync(Address updatedAddress);

        Task DeleteAddressAsync(Address address);
    }
}