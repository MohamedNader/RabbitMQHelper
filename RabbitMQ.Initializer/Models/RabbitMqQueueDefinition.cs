namespace RabbitMQ.Initializer.Models;

public sealed class RabbitMqQueueDefinition
{
    public string ExchangeName { get; init; }
    public string QueueName { get; init; } 
    public string RoutingKey { get; init; }
    public bool Durable { get; init; }
    public bool Exclusive { get; init; }
    public bool AutoDelete { get; init; }
    public string QueueType { get; init; }
}
