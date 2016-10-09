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
            _typeConverter = GetConverter(boundProperty);
            _headerKey     = headerKey;
        }

        private TypeConverter GetConverter(PropertyInfo boundProperty) {
            TypeConverter typeConverter;
            var tcAttr = _boundProperty.GetCustomAttribute<TypeConverterAttribute>();
            if (tcAttr != null) {
                var t = Type.GetType(tcAttr.ConverterTypeName);
                typeConverter = (TypeConverter)Activator.CreateInstance(t);
            }
            else {
                typeConverter = TypeDescriptor.GetConverter(boundProperty.PropertyType);
            }

            return typeConverter;
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