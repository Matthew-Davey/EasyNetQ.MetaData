# EasyNetQ.MetaData
An extension to EasyNetQ that allows you to utilize message headers and properties, without resorting to AdvancedBus! Even works with AutoSubscriber.

[![Build Status](https://travis-ci.org/Matthew-Davey/EasyNetQ.MetaData.svg?branch=develop)](https://travis-ci.org/Matthew-Davey/EasyNetQ.MetaData) [![Nuget Downloads](https://img.shields.io/nuget/dt/EasyNetQ.MetaData.svg)](https://www.nuget.org/packages/EasyNetQ.MetaData/) [![Nuget Version](https://img.shields.io/nuget/v/EasyNetQ.MetaData.svg)](https://www.nuget.org/packages/EasyNetQ.MetaData/)

### Getting Started
* Install the latest version of EasyNetQ.MetaData and EasyNetQ.MetaData.Abstractions from NuGet - `Install-Package EasyNetQ.MetaData; Install-Package EasyNetQ.MetaData.Abstractions`

### Message Headers
* Decorate the properties on your message POCO which you would like to bind to message headers

```csharp
using EasyNetQ.MetaData.Abstractions;
using System.Runtime.Serialization;

public class MyMessage
{
    public string MessageContent { get; set; }

    [MessageHeader("header_value"), IgnoreDataMember]
    public string HeaderValue { get; set; }
}
```

_You'll notice there's also an `IgnoreDataMember` attribute there. This is to stop the property showing up in the message body_ \*

* Enable message header binding when creating your message bus (this needs to be done on *both* producer and consumer sides)

```csharp 
using EasyNetQ.MetaData;

var bus = RabbitHutch.CreateBus(connectionString, registrar =>
    registrar.EnableMessageMetaDataBinding());

bus.Publish(new MyMessage {
    MessageContent = "Message Content",
    HeaderValue = "Header Value"
});
```

* You should now find that when you publish/send your message POCO, the `HeaderValue` property is no longer in the message body JSON\*\* but magically appears in the message headers under the key 'header_value'.

![RabbitMQ message header](https://cloud.githubusercontent.com/assets/2029369/11700473/fea41a0a-9ec1-11e5-8756-d2a5d4b1e20a.png)

And when you consume/receive the message, the `HeaderValue` property will be populated once again with the value from the header. Super!

Your properties can be of any type which is convertible to and from **string**. That is, any type for which `TypeDescriptor.GetTypeConverter` returns a TypeConverter which is capable of converting to and from **string**. This means you can serialize any complex type you like into the message header, providing that you are willing to write a TypeConverter for it. .NET Framework primitives and simple types (string, int, bool, DateTime, Guid, enums etc) are all supported for free!

\* _You will need a reference to `System.Runtime.Serialization`. I wish I could be really clever and make MessageHeaderAttribute extend IgnoreDataMemberAttribute, alas it is **sealed** :(_

\*\* _In theory this should all still work even if you have swapped out the default JsonSerializer for something more fancy (or less fancy, like XML)._

##### Uses for message headers
Perhaps you want to attach a routing-slip to the message, or a retry count, or a claim check, and you don't want these things to be in the message body. Perhaps you have built some extensions for EasyNetQ, like an `IProduceConsumeInterceptor` or an `IConsumerErrorStrategy`, and those extensions need to know a little bit about the message without having to deserialize it. Now you have a simple way of exposing interesting data to those extensions via message headers.

### Message Properties
In addition to message headers you can also bind one or more message properties...

```csharp
public class MyMessage {
    [MessageProperty(Property.ContentType), IgnoreDataMember]
    public String ContentType { get; set; }

    [MessageProperty(Property.ContentEncoding), IgnoreDataMember]
    public String ContentEncoding { get; set; }

    [MessageProperty(Property.Timestamp), IgnoreDataMember]
    public DateTime TimeStamp { get; set; }

    [MessageProperty(Property.DeliveryMode), IgnoreDataMember]
    public DeliveryMode DeliveryMode { get; set; }

    [MessageProperty(Property.Priority), IgnoreDataMember]
    public Byte Priority { get; set; }

    [MessageProperty(Property.CorrelationId), IgnoreDataMember]
    public Guid CorrelationId { get; set; }

    [MessageProperty(Property.ReplyTo), IgnoreDataMember]
    public String ReplyTo { get; set; }

    [MessageProperty(Property.Expiration), IgnoreDataMember]
    public TimeSpan Expiration { get; set; }

    [MessageProperty(Property.MessageId), IgnoreDataMember]
    public Guid MessageId { get; set; }
}
```

* **ContentType** - If you override this property you'll need to be sure to implement your own message `ISerializer` which reads & honours the setting.
* **ContentEncoding** - ^As above. You can override this to use an alternative encoding to UTF-8, if you want to.
* **Timestamp** - This must be a `DateTime` property (`DateTimeOffset` is not supported). It will be serialized in the message as a unix timestamp. This can be useful for calculating message handle time metrics.
* **DeliveryMode** - This can be any property which converts to `Byte`, although I recommend using the provided `DeliveryMode` enum for clarity. Use this to publish/send messages as non-persistent - useful if you're having IOps issues on your cluster.
* **Priority** - This can be any property which converts to `Byte`. This will only work for priority queues (https://www.rabbitmq.com/priority.html), which EasyNetQ does not declare. As such it's _probably useless_.
* **CorrelationId** - This has to be a `Guid`. EasyNetQ already assigns a CorrelationId but you can override it if you need to manually correlate messages together. May be useful for DTP?
* **ReplyTo** - You can put an exchange/queue name in here. You can use it on the consuming side to send a response message, although you're probably better off using EasyNetQ's built-in request/response capability.
* **Expiration** - This has to be a `TimeSpan` property. Use this to set a per-message TTL on your message (this has caveats - https://www.rabbitmq.com/ttl.html).
* **MessageId** - This has to be a `Guid`. Can't think of a good use for this but hey, it's there if you want it.


### Building from source
* Requires Ruby v2.1.7 (not compatible with 2.2.3). If you need to run multiple Ruby versions use RVM (https://rvm.io/). On Windows use RubyInstaller (http://rubyinstaller.org/)
* Requires Bundler (http://bundler.io/) `gem install bundler`

```
bundler install
bundle exec rake build [configuration=Debug|Release]
```

### Roadmap
* Support binding public fields in addition to properties (is there demand for this?).
