namespace EasyNetQ.MetaData.Bindings {
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Text;

    class HeaderBinding : IMetaDataBinding {
        public PropertyInfo BoundProperty { get; set; }
        public String HeaderKey { get; set; }

        public void ToMessageMetaData(Object source, MessageProperties destination) {
            var typeConverter = TypeDescriptor.GetConverter(BoundProperty.PropertyType);
            var propertyValue = BoundProperty.GetValue(source);

            if (propertyValue != null) {
                var headerValue   = typeConverter.ConvertToInvariantString(propertyValue);

                destination.Headers[HeaderKey] = headerValue;
            }
        }

        public void FromMessageMetaData(MessageProperties source, Object destination) {
            if (source.Headers.ContainsKey(HeaderKey)) {
                var headerBytes       = (Byte[])source.Headers[HeaderKey];
                var headerStringValue = Encoding.UTF8.GetString(headerBytes);
                var typeConverter     = TypeDescriptor.GetConverter(BoundProperty.PropertyType);
                var propertyValue     = typeConverter.ConvertFromInvariantString(headerStringValue);

                BoundProperty.SetValue(destination, propertyValue);
            }
        }
    }
}