namespace EasyNetQ.MetaData.Bindings {
    using System;
    using System.ComponentModel;
    using System.Reflection;

    class ContentTypeBinding : IMetaDataBinding {
        public PropertyInfo BoundProperty { get; set; }

        public void ToMessageMetaData(Object source, MessageProperties destination) {
            var typeConverter = TypeDescriptor.GetConverter(BoundProperty.PropertyType);
            var propertyValue = BoundProperty.GetValue(source);
            if (propertyValue != null) {
                var contentType = typeConverter.ConvertToInvariantString(propertyValue);

                destination.ContentTypePresent = true;
                destination.ContentType = contentType;
            }
        }

        public void FromMessageMetaData(MessageProperties source, Object destination) {
            if (source.ContentTypePresent) {
                var contentType = source.ContentType;
                var typeConverter = TypeDescriptor.GetConverter(BoundProperty.PropertyType);
                var propertyValue = typeConverter.ConvertFromInvariantString(contentType);

                BoundProperty.SetValue(destination, propertyValue);
            }
        }
    }
}