namespace EasyNetQ.MetaData.Bindings {
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Text;

    class HeaderBinding : IMetaDataBinding {
        private readonly PropertyInfo _boundProperty;
        private readonly TypeConverter _typeConverter;
        private readonly String _headerKey;

        public HeaderBinding(PropertyInfo boundProperty, String headerKey) {
            _boundProperty = boundProperty;
            _typeConverter = TypeDescriptor.GetConverter(boundProperty.PropertyType);
            _headerKey     = headerKey;
        }

        public void ToMessageMetaData(Object source, MessageProperties destination) {
            var propertyValue = _boundProperty.GetValue(source);

            if (propertyValue != null) {
                var headerValue = _typeConverter.ConvertToInvariantString(propertyValue);

                destination.Headers[_headerKey] = headerValue;
            }
        }

        public void FromMessageMetaData(MessageProperties source, Object destination) {
            if (source.Headers.ContainsKey(_headerKey)) {
                var headerBytes       = (Byte[])source.Headers[_headerKey];
                var headerStringValue = Encoding.UTF8.GetString(headerBytes);
                var propertyValue     = _typeConverter.ConvertFromInvariantString(headerStringValue);

                _boundProperty.SetValue(destination, propertyValue);
            }
        }
    }
}