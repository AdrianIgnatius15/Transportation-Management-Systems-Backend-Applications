using Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Address;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Models.Enums;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Order
{
    public record OrderUpdateDto
    {
        public OrderStatus? Status { get; set; }

        public string? Priority { get; set; }

        public AddressUpdateDto ShipmentAddress { get; set; } = new AddressUpdateDto();

        public AddressUpdateDto DeliveryAddress { get; set; } = new AddressUpdateDto();
    }
}
