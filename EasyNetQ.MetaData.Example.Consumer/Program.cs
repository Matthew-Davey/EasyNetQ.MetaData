namespace EasyNetQ.MetaData.Example.Consumer {
    using System;
    using EasyNetQ.Loggers;
    using EasyNetQ.MetaData.Example.Message;

    class Program {
        static void Main() {
            var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest", registrar => registrar
                .Register<IEasyNetQLogger>(_ => new ConsoleLogger())
                .EnableMessageMetaDataBinding()
            );

            Console.CancelKeyPress += (sender, eventArgs) => bus.Dispose();

            bus.Subscribe<ExampleEvent>(String.Empty, message => {
                Console.WriteLine("Custom Header Value: \"{0}\"", message.CustomHeaderValue);
                Console.WriteLine("Content Type:        \"{0}\"", message.ContentType);
                Console.WriteLine("Content Encoding:    \"{0}\"", message.ContentEncoding);
                Console.WriteLine("Message Timestamp:   \"{0}\"", message.Timestamp);
                Console.WriteLine("Delivery Mode:       \"{0}\"", message.DeliveryMode);
                Console.WriteLine("Priority:            \"{0}\"", message.Priority);
                Console.WriteLine("CorrelationId:       \"{0}\"", message.CorrelationId);
            });
        }
    }
}
