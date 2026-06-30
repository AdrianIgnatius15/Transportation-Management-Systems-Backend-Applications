using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Transportation_Management_System_Tracking_Service_REST_API.Data.Interfaces;
using Transportation_Management_System_Tracking_Service_REST_API.DTOs;
using Transportation_Management_System_Tracking_Service_REST_API.DTOs.MQ;
using Transportation_Management_System_Tracking_Service_REST_API.Middlewares.Interfaces;
using Transportation_Management_System_Tracking_Service_REST_API.Models;
using Transportation_Management_System_Tracking_Service_REST_API.Models.Enums;
using Transportation_Management_System_Tracking_Service_REST_API.SignalRControllers;
using Transportation_Management_System_Tracking_Service_REST_API.Utilities;

namespace Transportation_Management_System_Tracking_Service_REST_API.Controllers
{
    [ApiController]
    [Route("api/tracking")]
    public class TrackingController : ControllerBase
    {
        private readonly ITrackingEventRepo _repo;
        private readonly IMQProducer _mqProducer;
        private readonly IHubContext<TrackingHubController> _hub;

        public TrackingController(
            ITrackingEventRepo repo,
            IMQProducer mqProducer,
            IHubContext<TrackingHubController> hub
        )
        {
            _repo = repo;
            _mqProducer = mqProducer;
            _hub = hub;
        }

        public async Task<ActionResult> LogEvent(
            Guid orderId,
            [FromBody] TrackingEventCreateDto createDto
        )
        {
            var trackingEventEntity = MapperUtility.Map<TrackingEventCreateDto, TrackingEvent>(
                createDto
            );
            var trackingEvent = await _repo.AddEventAsync(orderId, trackingEventEntity);
            bool savedStatus = await _repo.SaveChangesAsync();

            if (savedStatus)
            {
                await _hub
                    .Clients.Group($"order-{orderId}")
                    .SendAsync("TrackingUpdated", trackingEvent);

                if (createDto.EventType != TrackingEventType.PositionUpdate)
                {
                    await _mqProducer.SendMessage(
                        new TrackingEventPublishedEvent(
                            orderId,
                            createDto.EventType,
                            createDto.Description,
                            trackingEvent.Timestamp
                        )
                    );

                    return Ok(trackingEvent);
                }
            }

            return NoContent();
        }

        [HttpGet("{orderId:guid}/events")]
        // [Authorize(Roles = "shipper,driver")]
        public async Task<ActionResult> GetEvents(Guid orderId) =>
            Ok(await _repo.GetTrackingEventHistoryAsync(orderId));

        [HttpGet("{orderId:guid}/milestones")]
        // [Authorize(Roles = "shipper,driver")]
        public async Task<ActionResult> GetMilestones(Guid orderId) =>
            Ok(await _repo.GetMilestoneHistoryEventAsync(orderId));

        [HttpGet("{orderId:guid}/live")]
        // [Authorize(Roles = "shipper,driver")]
        public async Task<ActionResult> GetLivePosition(Guid orderId)
        {
            var latest = await _repo.GetLatestPositionAsync(orderId);
            return latest != null ? Ok(latest) : NotFound("No position data yet.");
        }
    }
}
