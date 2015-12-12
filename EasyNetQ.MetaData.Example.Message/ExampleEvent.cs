namespace EasyNetQ.MetaData.Example.Message {
    using System;
    using System.Runtime.Serialization;

    public class ExampleEvent {
        public String MessageContent { get; set; }

        [MessageHeader("my_custom_header"), IgnoreDataMember]
        public String HeaderValue { get; set; }
    }
}