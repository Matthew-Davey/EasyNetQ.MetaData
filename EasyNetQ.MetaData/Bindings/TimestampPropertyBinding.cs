namespace EasyNetQ.MetaData.Bindings {
    using System;
    using System.Reflection;

    class TimestampPropertyBinding : IMetaDataBinding {
        public PropertyInfo BoundProperty { get; set; }

        public void ToMessageMetaData(Object source, MessageProperties destination) {
            var propertyValue = BoundProperty.GetValue(source);
            var timestamp = (DateTime)Convert.ChangeType(propertyValue, typeof(DateTime));

            destination.TimestampPresent = true;
            destination.Timestamp = timestamp.ToUnixTimestamp();
        }

        public void FromMessageMetaData(MessageProperties source, Object destination) {
            var timestamp = source.Timestamp.FromUnixTimestamp();
            var propertyValue = Convert.ChangeType(timestamp, BoundProperty.PropertyType);

            BoundProperty.SetValue(destination, propertyValue);
        }
    }
}