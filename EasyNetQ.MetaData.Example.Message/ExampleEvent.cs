namespace EasyNetQ.MetaData.Example.Message {
    using System;
    using System.Runtime.Serialization;

    public class ExampleEvent {
        public String MessageContent { get; set; }

        [IgnoreDataMember]
        [MessageHeader("header_value")]
        public String HeaderValue { get; set; }
    }
}