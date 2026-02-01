using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Helper.Core;

public interface IMessageBus
{
    void Publish<T>(T @event, string exchangeName, string routingKey) where T : IEvent;
    void Subscribe<T, TH>(string queueName, string exchangeName, string routingKey)
        where T : IEvent
        where TH : IEventHandler<T>;
}
