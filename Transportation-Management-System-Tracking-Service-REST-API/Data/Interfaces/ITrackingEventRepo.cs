using Transportation_Management_System_Tracking_Service_REST_API.DTOs;
using Transportation_Management_System_Tracking_Service_REST_API.Models;

namespace Transportation_Management_System_Tracking_Service_REST_API.Data.Interfaces
{
    public interface ITrackingEventRepo
    {
        Task<TrackingEvent> AddEventAsync(Guid orderId, TrackingEventCreateDto trackingEvent);

        Task<IEnumerable<TrackingEvent>> GetTrackingEventHistoryAsync(Guid orderId);

        Task<IEnumerable<TrackingEvent>> GetMilestoneHistoryEventAsync(Guid orderId);

        Task<TrackingEvent?> GetLatestPositionAsync(Guid orderId);

        Task<bool> SaveChangesAsync();
    }
}
