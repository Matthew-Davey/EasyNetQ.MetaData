# EasyNetQ.MetaData
An extension to EasyNetQ that allows you to utilize message headers, without resorting to AdvancedBus! Even works with AutoSubscriber.

[![Build Status](https://travis-ci.org/Matthew-Davey/EasyNetQ.MetaData.svg?branch=develop)](https://travis-ci.org/Matthew-Davey/EasyNetQ.MetaData)

### Getting Started
* Install the latest version of EasyNetQ.MetaData from NuGet - `Install-Package EasyNetQ.MetaData`
* Decorate the properties on your message POCO which you would like to bind to message headers
```csharp
using EasyNetQ.MetaData;
using System.Runtime.Serialization;

public class MyMessage
{
    public string MessageContent { get; set; }

    [MessageHeader("my_message_header"), IgnoreDataMember]
    public string HeaderValue { get; set; }
}
```

_You'll notice there's also an `IgnoreDataMember` attribute there. This is to stop the property showing up in the message body_ \*

* Enable message header binding when creating your message bus (this needs to be done on *both* producer and consumer sides)
```csharp 
using EasyNetQ.MetaData;

var bus = RabbitHutch.CreateBus(connectionString, registrar =>
    registrar.EnableMessageMetaDataBinding());
```

* You should now find that when you publish/send your message POCO, the `HeaderValue` property is no longer in the message body JSON\*\* but magically appears in the message headers under the key 'my_message_header'. And when you consume/receive the message, the `HeaderValue` property will be populated once again with the value from the header. Super!

Your properties can be of any type which is convertible to and from **string**. That is, any type for which `TypeDescriptor.GetTypeConverter` returns a TypeConverter which is capable of converting to and from **string**. This means you can serialize any complex type you like into the message header, providing that you are willing to write a TypeConverter for it. .NET Framework primitives and simple types (string, int, bool, DateTime, Guid, enums etc) are all supported for free!

\* _You will need a reference to `System.Runtime.Serialization`. I wish I could be really clever and make MessageHeaderAttribute extend IgnoreDataMemberAttribute, alas it is **sealed** :(_

\*\* _In theory this should all still work even if you have swapped out the default JsonSerializer for something more fancy (or less fancy, like XML)._

### Building from source
```
bundler install
bundle exec rake build [configuration=Debug|Release]
```

### Roadmap
* Special handling of DateTime objects. Currently these are serialized to the message header in **local** format. It would be preferable to serialize them in ISO-8601 format, or as unix timestamps.
* Support binding public fields in addition to properties (is there demand for this?).
* Support binding to AMQP message properties.
