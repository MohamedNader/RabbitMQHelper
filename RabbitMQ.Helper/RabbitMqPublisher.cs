using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Helper.Core;
using RabbitMQ.Helper.Models;
using System;
using System.Text.Json;

namespace RabbitMQ.Helper;

public class RabbitMqPublisher : IRabbitMqPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly IServiceProvider _serviceProvider;

    public RabbitMqPublisher(IOptions<RabbitMqOptions> options, IServiceProvider serviceProvider)
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

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        _serviceProvider = serviceProvider;
    }

    public async Task Publish<T>(T @event, string exchange, string routingKey) where T : IEvent
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(@event);
        await _channel.BasicPublishAsync(exchange, routingKey,body);
    }

    public async Task Subscribe<T, TH>(string queue, string exchange, string routingKey)
        where T : IEvent where TH : IEventHandler<T>
    {
        await _channel.QueueDeclareAsync(queue, durable: true, exclusive: false, autoDelete: false);
        await _channel.QueueBindAsync(queue, exchange, routingKey);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var @event = JsonSerializer.Deserialize<T>(body);

            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<TH>();
            await handler.HandleAsync(@event);

            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        };

        await _channel.BasicConsumeAsync(queue, false, consumer);
    }

    public void Dispose()
    {
        _channel?.CloseAsync().GetAwaiter().GetResult();
        _connection?.CloseAsync().GetAwaiter().GetResult();
    }
}