using Microsoft.AspNetCore.Mvc;
using Transportation_Management_System_Tracking_Service_REST_API.Data.Interfaces;
using Transportation_Management_System_Tracking_Service_REST_API.DTOs;

namespace Transportation_Management_System_Tracking_Service_REST_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriverController : ControllerBase
    {
        private readonly ITrackingEventRepo _trackingEventRepo;
        private readonly IOrderRepo _orderRepo;

        public DriverController(ITrackingEventRepo trackingEventRepo, IOrderRepo orderRepo)
        {
            _trackingEventRepo = trackingEventRepo;
            _orderRepo = orderRepo;
        }

        [HttpPut("update-order-shipment-event/{id}")]
        public async Task<ActionResult> UpdateOrderShipmentTrackingEvent(Guid id, [FromBody] TrackingEventUpdateDto trackingEventUpdateDto)
        {
            if (id.ToString() == "" || id.ToString().Length == 0)
            {
                return BadRequest("There is no ID of the order shipment supplied. Please send the order shipment ID");
            }
            else
            {
                _trackingEventRepo.UpdateExistingOrderShipmentEvent(id, trackingEventUpdateDto);
                await _trackingEventRepo.SaveChangesAsync();
                // Create a MQ publisher and the service for it using RabbitMQ and publish message for periodic updates of the order status.
                return NoContent();
            }
        }
    }
}