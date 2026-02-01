using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Helper;
using RabbitMQ.Helper.Core;
using RabbitMQ.Helper.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Initializer;

public class RabbitMQRegisteration
{
    public static void RegisterRabbitMQHandler(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));


        serviceCollection.AddSingleton<RabbitMqInitializer>();
        serviceCollection.AddSingleton<IRabbitMqPublisher ,RabbitMqPublisher>();
    }
}
