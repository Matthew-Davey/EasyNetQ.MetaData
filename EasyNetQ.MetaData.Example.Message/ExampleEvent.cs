using System;

namespace EasyNetQ.MetaData.Example.Message {
    public class ExampleEvent {
        public String MessageContent { get; set; }

        [MessageHeader("header_value")]
        public String HeaderValue { get; set; }
    }
}