using Transportation_Management_System_Tracking_Service_REST_API.Models;

namespace Transportation_Management_System_Tracking_Service_REST_API.Data.Interfaces
{
    public interface IOrderRepo
    {
        Task<Order?> GetOrderShipmentById(Guid id);
    }
}