using System;

namespace Transportation_Management_System_Tracking_Service_REST_API.Models.Config;

public class QueueConfiguration
{
    public string Id { get; set; } = string.Empty;
    public string ExchangeName { get; set; } = string.Empty;
    public string QueueName { get; set; } = string.Empty;
    public string RoutingKeyPattern { get; set; } = string.Empty;
}
