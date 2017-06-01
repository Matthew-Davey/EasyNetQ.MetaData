namespace EasyNetQ.MetaData.Example.Message {
    using System;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using EasyNetQ.MetaData.Abstractions;

    public class ExampleEvent {
        public String MessageContent { get; set; }

        [MessageHeader("my_custom_header"), IgnoreDataMember]
        public String CustomHeaderValue { get; set; }

        [MessageHeader("my_custom_date_header"), IgnoreDataMember, TypeConverter(typeof(SampleDateConverter))]
        public DateTime CustomHeaderDateValue { get; set; }

        [MessageProperty(Property.ContentType), IgnoreDataMember]
        public String ContentType { get; set; }

        [MessageProperty(Property.ContentEncoding), IgnoreDataMember]
        public String ContentEncoding { get; set; }

        [MessageProperty(Property.Timestamp), IgnoreDataMember]
        public DateTime Timestamp { get; set; }

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
}