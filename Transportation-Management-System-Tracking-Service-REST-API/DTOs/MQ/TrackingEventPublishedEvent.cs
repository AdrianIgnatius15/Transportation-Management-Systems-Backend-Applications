using Transportation_Management_System_Tracking_Service_REST_API.Models.Enums;

namespace Transportation_Management_System_Tracking_Service_REST_API.DTOs.MQ
{
    public record TrackingEventPublishedEvent
    (
        Guid OrderId,
        TrackingEventType EventType,
        string? Description,
        DateTime TimeStamp
    );
}