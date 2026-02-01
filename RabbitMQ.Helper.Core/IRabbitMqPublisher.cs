using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Helper.Core;

public interface IRabbitMqPublisher
{
    Task Publish<T>(T @event, string exchange, string routingKey) where T : IEvent;
    Task Subscribe<T, TH>(string queue, string exchange, string routingKey)
    where T : IEvent where TH : IEventHandler<T>;

}
