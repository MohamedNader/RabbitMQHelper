using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Helper.Models
{
    public sealed class RabbitMqOptions
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public string ClientProvidedName { get; set; }
    }
}
