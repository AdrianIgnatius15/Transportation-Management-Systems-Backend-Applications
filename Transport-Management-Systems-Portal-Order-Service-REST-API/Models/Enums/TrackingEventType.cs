namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Models.Enums
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
        Exception
    }
}