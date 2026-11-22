using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Transportation_Management_System_Tracking_Service_REST_API.DTOs;
using Transportation_Management_System_Tracking_Service_REST_API.DTOs.Events;
using Transportation_Management_System_Tracking_Service_REST_API.Middlewares.Interfaces;

namespace Transportation_Management_System_Tracking_Service_REST_API.SignalRControllers
{
    [Authorize]
    public class TrackingHubController : Hub
    {
        private readonly IMQProducer _mqProducer;

        public TrackingHubController(IMQProducer mQProducer)
        {
            _mqProducer = mQProducer;
        }

        public async Task JointOrderTracking(string orderId, CancellationToken cancellationToken)
            => await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(orderId), cancellationToken);

        public async Task LeaveOrderTracking(string orderId, CancellationToken cancellationToken)
            => await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(orderId), cancellationToken);

        private static string GroupName(string orderId)
            => $"order-{orderId}";

        public async Task ReceiveOrderShipmentUpdatesFromDriver(string orderId, TrackingEventCreateDto trackingEventCreateDto)
        {
            var driverUserId = Context.UserIdentifier ?? Context.ConnectionId;
            Console.WriteLine($"Driver ID: {driverUserId}");
            // await Clients.Group(GroupName(orderId)).SendAsync("ReceiveOrderShipmentLocation", trackingEventCreateDto);
            var trackingEvent = new TrackingEventPublishedEvent(
                Guid.Parse(orderId),
                trackingEventCreateDto.EventType,
                trackingEventCreateDto.Description,
                trackingEventCreateDto.Latitude,
                trackingEventCreateDto.Longitude,
                new DateTime()
            );
            await _mqProducer.SendMessage<TrackingEventPublishedEvent>(trackingEvent);
        }
    }
}