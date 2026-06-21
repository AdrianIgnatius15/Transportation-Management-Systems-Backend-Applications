using RabbitMQ.Client;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Middlewares.Interfaces;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Middlewares
{
    public class RabbitMQConnectionMiddleware : IRabbitMQConnection, IDisposable
    {
        private readonly ConnectionFactory _factory;
        private IConnection? _connection;
        private readonly SemaphoreSlim _connectionLock = new(1, 1);

        public RabbitMQConnectionMiddleware(IConfiguration configuration)
        {
            _factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"] ?? "tms-mq",
                UserName = configuration["RabbitMQ:Username"] ?? "tms",
                Password = configuration["RabbitMQ:Password"] ?? "tms123"
            };
        }

        public async Task<IConnection> GetConnectionAsync()
        {
            if (_connection is { IsOpen: true }) return _connection;

            await _connectionLock.WaitAsync();
            try
            {
                if (_connection is not { IsOpen: true })
                {
                    _connection = await _factory.CreateConnectionAsync();
                }
                return _connection;
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public void Dispose() => _connection?.Dispose();
    }
}