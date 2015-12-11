namespace EasyNetQ.MetaData.Example.Message {
    using System;
    using System.Runtime.Serialization;

    public class ExampleEvent {
        public String MessageContent { get; set; }

        [MessageHeader("header_value"), IgnoreDataMember]
        public String HeaderValue { get; set; }

        [MessageProperty(Property.Timestamp), IgnoreDataMember]
        public DateTimeOffset Timestamp { get; set; }
    }
}