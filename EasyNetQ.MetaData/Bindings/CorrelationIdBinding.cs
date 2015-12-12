namespace EasyNetQ.MetaData.Bindings {
    using System;
    using System.ComponentModel;
    using System.Reflection;

    class CorrelationIdBinding : IMetaDataBinding {
        public PropertyInfo BoundProperty { get; set; }

        public void ToMessageMetaData(Object source, MessageProperties destination) {
            var typeConverter = TypeDescriptor.GetConverter(BoundProperty.PropertyType);
            var propertyValue = BoundProperty.GetValue(source);
            
            if (propertyValue != null) {
                var correlationId = typeConverter.ConvertToInvariantString(propertyValue);

                destination.CorrelationIdPresent = true;
                destination.CorrelationId = correlationId;
            }
        }

        public void FromMessageMetaData(MessageProperties source, Object destination) {
            if (source.CorrelationIdPresent) {
                var correlationId = source.CorrelationId;
                var typeConverter = TypeDescriptor.GetConverter(BoundProperty.PropertyType);
                var propertyValue = typeConverter.ConvertFromInvariantString(correlationId);

                BoundProperty.SetValue(destination, propertyValue);
            }
        }
    }
}