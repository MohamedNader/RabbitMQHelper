using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Helper.Core;

public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    Task HandleAsync(TEvent @event);
}