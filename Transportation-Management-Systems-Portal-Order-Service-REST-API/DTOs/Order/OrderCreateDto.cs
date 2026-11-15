using System.ComponentModel.DataAnnotations;
using Transportation_Management_Systems_Portal_Order_Service_REST_API.DTOs.Address;
using Transportation_Management_Systems_Portal_Order_Service_REST_API.DTOs.Shipment;
using Transportation_Management_Systems_Portal_Order_Service_REST_API.Models.Enums;

namespace Transportation_Management_Systems_Portal_Order_Service_REST_API.DTOs.Order
{
    public record OrderCreateDto
    {
        [Required]
        public Guid ClientId { get; set; }

        [Required]
        public string OrderNumber { get; set; } = default!;

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Created;

        [Required]
        public string Priority { get; set; } = string.Empty;

        [Required]
        public AddressCreateDto ShipmentAddress { get; set; } = default!;

        [Required]
        public AddressCreateDto DeliveryAddress { get; set; } = default!;

        public List<ShipmentCreateDto> Shipments { get; set; } = new List<ShipmentCreateDto>();
    }
}
