namespace EasyNetQ.MetaData.Example.Producer {
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using EasyNetQ;
    using EasyNetQ.Loggers;
    using EasyNetQ.MetaData.Example.Message;

    class Program {
        static void Main() {
            var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest", registrar => registrar
                .Register<IEasyNetQLogger>(_ => new ConsoleLogger())
                .EnableMessageMetaDataBinding()
            );

            var cancellationTokenSource = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, eventArgs) => {
                cancellationTokenSource.Cancel();
                eventArgs.Cancel = true;
            };

            while (!cancellationTokenSource.IsCancellationRequested) {
                bus.Publish(new ExampleEvent {
                    MessageContent = "Message Content",
                    HeaderValue    = "Header Value",
                    Timestamp      = DateTimeOffset.UtcNow
                });

                Task.Delay(1000).Wait();
            }

            bus.Dispose();
        }
    }
}
