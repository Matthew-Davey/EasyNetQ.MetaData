# EasyNetQ.MetaData
An extension to EasyNetQ that allows you to utilize message headers, without resorting to AdvancedBus! Even works with AutoSubscriber.

# Getting Started
1. Install the latest version of EasyNetQ.MetaData from NuGet - `Install-Package EasyNetQ.MetaData`
2. Decorate the properties on your message POCO which you would like to bind to message headers
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

3. Enable message header binding when creating your message bus (this needs to be done on *both* producer and consumer sides)
```csharp 
using EasyNetQ.MetaData;

var bus = RabbitHutch.CreateBus(connectionString, registrar => registrar
    registrar.EnableMessageMetaDataBinding());
```

4. You should now find that when you publish/send your message POCO, the `HeaderValue` property is no longer in the message body JSON but magically appears in the message headers under the key 'my_message_header'. And when you consume/receive the message, the `HeaderValue` property will be populated once again with the value from the header. Super!

\* You will need a reference to `System.Runtime.Serialization`. I wish I could be really clever and make `MessageHeaderAttribute` extend `IgnoreDataMemberAttribute`, alas it is **sealed** :(

# Building from source
```
bundler install
bundle exec rake build [configuration=Debug|Release]
```