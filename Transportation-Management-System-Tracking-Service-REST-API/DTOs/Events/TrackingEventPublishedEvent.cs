using Transportation_Management_System_Tracking_Service_REST_API.Models.Enums;

namespace Transportation_Management_System_Tracking_Service_REST_API.DTOs.Events
{
    public record TrackingEventPublishedEvent(
        Guid OrderId,
        TrackingEventType EventType,
        string? Description,
        double? Latitude,
        double? Longitude,
        DateTime Timestamp
    )
    {
        public Guid OrderId { get; set; } = OrderId;
        public TrackingEventType EventType { get; set; } = EventType;
        public string? Description { get; set; } = Description;
        public double? Latitude { get; set; } = Latitude;
        public double? Longitude { get; set; } = Longitude;
        public DateTime Timestamp { get; set; } = Timestamp;
    };
}
