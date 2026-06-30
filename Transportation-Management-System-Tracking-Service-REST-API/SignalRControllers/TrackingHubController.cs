using Microsoft.AspNetCore.SignalR;

namespace Transportation_Management_System_Tracking_Service_REST_API.SignalRControllers
{
    public class TrackingHubController : Hub
    {
        public async Task JointOrderTracking(string orderId)
            => await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(orderId));

        public async Task LeaveOrderTracking(string orderId)
            => await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(orderId));

        private static string GroupName(string orderId)
            => $"order-{orderId}";
    }
}