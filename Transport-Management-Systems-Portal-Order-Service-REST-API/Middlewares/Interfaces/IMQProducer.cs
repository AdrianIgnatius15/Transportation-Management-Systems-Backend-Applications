namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Middlewares.Interfaces
{
    public interface IMQProducer
    {
        Task SendMessage<T> (T message);
    }
}