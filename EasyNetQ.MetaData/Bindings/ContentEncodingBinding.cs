namespace EasyNetQ.MetaData.Bindings {
    using System;
    using System.ComponentModel;
    using System.Reflection;

    class ContentEncodingBinding : IMetaDataBinding {
        public PropertyInfo BoundProperty { get; set; }

        public void ToMessageMetaData(Object source, MessageProperties destination) {
            var typeConverter = TypeDescriptor.GetConverter(BoundProperty.PropertyType);
            var propertyValue = BoundProperty.GetValue(source);
            var contentEncoding = typeConverter.ConvertToInvariantString(propertyValue);

            destination.ContentEncodingPresent = true;
            destination.ContentEncoding = contentEncoding;
        }

        public void FromMessageMetaData(MessageProperties source, Object destination) {
            var contentEncoding = source.ContentEncoding;
            var typeConverter = TypeDescriptor.GetConverter(BoundProperty.PropertyType);
            var propertyValue = typeConverter.ConvertFromInvariantString(contentEncoding);

            BoundProperty.SetValue(destination, propertyValue);
        }
    }
}