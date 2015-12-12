namespace EasyNetQ.MetaData.Bindings {
    using System;
    using System.ComponentModel;
    using System.Reflection;

    class MessageIdBinding : IMetaDataBinding {
        public PropertyInfo BoundProperty { get; set; }

        public void ToMessageMetaData(Object source, MessageProperties destination) {
            var typeConverter = TypeDescriptor.GetConverter(BoundProperty.PropertyType);
            var propertyValue = BoundProperty.GetValue(source);
            var messageId = typeConverter.ConvertToInvariantString(propertyValue);

            destination.MessageIdPresent = true;
            destination.MessageId = messageId;
        }

        public void FromMessageMetaData(MessageProperties source, Object destination) {
            var messageId = source.MessageId;
            var typeConverter = TypeDescriptor.GetConverter(BoundProperty.PropertyType);
            var propertyValue = typeConverter.ConvertFromInvariantString(messageId);

            BoundProperty.SetValue(destination, propertyValue);
        }
    }
}