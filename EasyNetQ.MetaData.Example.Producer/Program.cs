namespace EasyNetQ.MetaData.Example.Producer {
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Text;
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
                    MessageContent    = "Message Content",
                    CustomHeaderValue = "My Header Value",
                    ContentType       = "application/json",
                    ContentEncoding   = Encoding.UTF8.WebName,
                    Timestamp         = DateTime.UtcNow,
                    DeliveryMode      = 1, // Non-persistent
                    Priority          = 1,
                    CorrelationId     = Guid.NewGuid(),
                    ReplyTo           = "Response Queue",
                    Expiration        = TimeSpan.FromSeconds(30),
                    MessageId         = Guid.NewGuid()
                });

                Task.Delay(1000).Wait();
            }

            bus.Dispose();
        }
    }
}
