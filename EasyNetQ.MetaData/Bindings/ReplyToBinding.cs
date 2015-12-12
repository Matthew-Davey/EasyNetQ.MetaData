namespace EasyNetQ.MetaData.Bindings {
    using System;
    using System.ComponentModel;
    using System.Reflection;

    class ReplyToBinding : IMetaDataBinding {
        public PropertyInfo BoundProperty { get; set; }

        public void ToMessageMetaData(Object source, MessageProperties destination) {
            var typeConverter = TypeDescriptor.GetConverter(BoundProperty.PropertyType);
            var propertyValue = BoundProperty.GetValue(source);

            if (propertyValue != null) {
                var replyTo = typeConverter.ConvertToInvariantString(propertyValue);

                destination.ReplyToPresent = true;
                destination.ReplyTo = replyTo;
            }
        }

        public void FromMessageMetaData(MessageProperties source, Object destination) {
            if (source.ReplyToPresent) {
                var replyTo = source.ReplyTo;
                var typeConverter = TypeDescriptor.GetConverter(BoundProperty.PropertyType);
                var propertyValue = typeConverter.ConvertFromInvariantString(replyTo);

                BoundProperty.SetValue(destination, propertyValue);
            }
        }
    }
}