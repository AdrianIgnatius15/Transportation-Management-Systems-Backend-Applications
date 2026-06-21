namespace Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Keycloak
{
    public class KeycloakEvent
    {
        public string Type { get; set; } = string.Empty;
        public string RealmId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public Dictionary<string, string> Details { get; set; } = [];
    }
}