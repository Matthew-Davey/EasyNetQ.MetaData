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
                // Intentionally empty...
            });
        }
    }
}
