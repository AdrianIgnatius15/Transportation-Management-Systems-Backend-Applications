using Microsoft.EntityFrameworkCore;
using Transportation_Management_System_Tracking_Service_REST_API.Data.Interfaces;
using Transportation_Management_System_Tracking_Service_REST_API.DTOs;
using Transportation_Management_System_Tracking_Service_REST_API.Models;
using Transportation_Management_System_Tracking_Service_REST_API.Models.Enums;

namespace Transportation_Management_System_Tracking_Service_REST_API.Data.Repositories
{
    public class TrackingRepo : ITrackingEventRepo
    {
        private readonly TMSDbContext _dbContext;

        public TrackingRepo(TMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TrackingEvent> AddEventAsync(
            Guid orderId,
            TrackingEventCreateDto trackingEvent
        )
        {
            var trackingEventToBeSaved = new TrackingEvent
            {
                OrderId = orderId,
                EventType = trackingEvent.EventType,
                Description = trackingEvent.Description,
                Latitude = trackingEvent.Latitude,
                Longitude = trackingEvent.Longitude,
                Timestamp = DateTime.UtcNow,
            };

            var savedTrackingEvent = await _dbContext.TrackingEvents.AddAsync(
                trackingEventToBeSaved
            );
            return savedTrackingEvent.Entity;
        }

        public async Task<TrackingEvent?> GetLatestPositionAsync(Guid orderId) =>
            await _dbContext
                .TrackingEvents.Where(e => e.OrderId == orderId)
                .OrderByDescending(e => e.Timestamp)
                .FirstOrDefaultAsync();

        public async Task<IEnumerable<TrackingEvent>> GetMilestoneHistoryEventAsync(Guid orderId) =>
            await _dbContext
                .TrackingEvents.Where(trackingEvent =>
                    trackingEvent.OrderId == orderId
                    && trackingEvent.EventType != TrackingEventType.PositionUpdate
                )
                .OrderBy(trackingEvent => trackingEvent.Timestamp)
                .ToListAsync();

        public async Task<IEnumerable<TrackingEvent>> GetTrackingEventHistoryAsync(Guid orderId) =>
            await _dbContext
                .TrackingEvents.Where(trackingEvent =>
                    trackingEvent.OrderId == orderId
                    && trackingEvent.Latitude != null
                    && trackingEvent.Longitude != null
                )
                .OrderBy(trackingEvent => trackingEvent.Timestamp)
                .ToListAsync();

        public async Task<bool> SaveChangesAsync()
        {
            return (await _dbContext.SaveChangesAsync()) > 0;
        }
    }
}
