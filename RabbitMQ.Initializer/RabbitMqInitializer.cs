using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Helper.Models;
using RabbitMQ.Initializer.Models;

namespace RabbitMQ.Initializer;


public sealed class RabbitMqInitializer : IDisposable
{
    private readonly IConnection _connection;

    public RabbitMqInitializer(IOptions<RabbitMqOptions> options)
    {
        var opts = options.Value;

        var factory = new ConnectionFactory
        {
            HostName = opts.HostName,
            Port = opts.Port,
            UserName = opts.UserName,
            Password = opts.Password,
            VirtualHost = opts.VirtualHost
        };

        _connection = factory.CreateConnectionAsync(opts.ClientProvidedName).GetAwaiter().GetResult();
    }

    public async Task InitializeQueues(IEnumerable<RabbitMqQueueDefinition> definitions)
    {
        using var channel = await _connection.CreateChannelAsync();

        var args = new Dictionary<string, object>
        {
            { "x-queue-type", "classic" } // Explicitly set it to Classic
        };
        foreach (var def in definitions)
        {
            await channel.ExchangeDeclareAsync(def.ExchangeName, ExchangeType.Direct, def.Durable);

            await channel.QueueDeclareAsync(
                def.QueueName,
                durable: def.Durable,
                exclusive: def.Exclusive,
                autoDelete: def.AutoDelete,
                arguments: args);

            await channel.QueueBindAsync(def.QueueName, def.ExchangeName, def.RoutingKey);
        }
    }

    public void Dispose() => _connection.Dispose();
}
