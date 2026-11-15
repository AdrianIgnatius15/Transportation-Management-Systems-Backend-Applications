using RabbitMQ.Client;

namespace Transportation_Management_Systems_Portal_Order_Service_REST_API.Middlewares.Interfaces
{
    public interface IRabbitMQConnection
    {
        Task<IConnection> GetConnectionAsync();
    }
}