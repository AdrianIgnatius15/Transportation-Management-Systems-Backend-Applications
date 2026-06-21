namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Models.Enums
{
    public enum OrderStatus
    {
        Created,
        ReadyForPickup,
        InTransit,
        OutForDelivery,
        Delivered,
        Failed,
        Cancelled
    }
}