namespace Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Shipment
{
    public record ShipmentUpdateDto
    {
        public Guid? OrderId { get; set; }
    }
}