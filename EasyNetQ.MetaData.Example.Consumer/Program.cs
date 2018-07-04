namespace EasyNetQ.MetaData.Example.Consumer {
    using System;
    using EasyNetQ;
    using EasyNetQ.Logging;
    using EasyNetQ.MetaData.Example.Message;

    class Program {
        static void Main() {
            LogProvider.SetCurrentLogProvider(ConsoleLogProvider.Instance);

            var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest", registrar =>
                registrar.EnableMessageMetaDataBinding()
            );

            Console.CancelKeyPress += (sender, EventArgs) => bus.Dispose();

            bus.Subscribe<ExampleEvent>(String.Empty, message => {
                Console.WriteLine($"Custom Header Value: \"{message.CustomHeaderValue}\"");
                Console.WriteLine($"Content Type:        \"{message.ContentType}\"");
                Console.WriteLine($"Content Encoding:    \"{message.ContentEncoding}\"");
                Console.WriteLine($"Message Timestamp:   \"{message.Timestamp}\"");
                Console.WriteLine($"Delivery Mode:       \"{message.DeliveryMode}\"");
                Console.WriteLine($"Priority:            \"{message.Priority}\"");
                Console.WriteLine($"CorrelationId:       \"{message.CorrelationId}\"");
                Console.WriteLine($"ReplyTo:             \"{message.ReplyTo}\"");
                Console.WriteLine($"Expiration:          \"{message.Expiration}\"");
                Console.WriteLine($"MessageId:           \"{message.MessageId}\"");
            });
        }
    }
}
