namespace Transportation_Management_System_Tracking_Service_REST_API.Models.Enums
{
    public enum TrackingEventType
    {
        Created,
        PickedUp,
        ArrivedAtFacility,
        DepartedFacility,
        InTransit,
        OutForDelivery,
        Delivered,
        FailedAttempt,
        Exception,
        PositionUpdate
    }
}