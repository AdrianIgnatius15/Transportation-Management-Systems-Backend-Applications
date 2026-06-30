namespace Transportation_Management_System_Tracking_Service_REST_API.Middlewares.Interfaces
{
    public interface IMQProducer
    {
        Task SendMessage<T> (T message);
    }
}