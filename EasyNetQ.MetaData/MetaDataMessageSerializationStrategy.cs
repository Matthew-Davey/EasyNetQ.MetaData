namespace EasyNetQ.MetaData {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal class MetaDataMessageSerializationStrategy : IMessageSerializationStrategy {
        private readonly ITypeNameSerializer _typeNameSerializer;
        private readonly ISerializer _serializer;
        private readonly ICorrelationIdGenerationStrategy _correlationIdGenerator;

        public MetaDataMessageSerializationStrategy(ITypeNameSerializer typeNameSerializer, ISerializer serializer, ICorrelationIdGenerationStrategy correlationIdGenerator) {
            if (typeNameSerializer == null)
                throw new ArgumentNullException("typeNameSerializer");

            if (serializer == null)
                throw new ArgumentNullException("serializer");

            if (correlationIdGenerator == null)
                throw new ArgumentNullException("correlationIdGenerator");

            _typeNameSerializer = typeNameSerializer;
            _serializer = serializer;
            _correlationIdGenerator = correlationIdGenerator;
        }

        public IMessage DeserializeMessage(MessageProperties properties, Byte[] body) {
            var messageType = _typeNameSerializer.DeSerialize(properties.Type);
            var messageBody = _serializer.BytesToMessage(properties.Type, body);

            BindHeaderValues(messageType, messageBody, properties.Headers);

            return MessageFactory.CreateInstance(messageType, messageBody, properties);
        }

        public IMessage<T> DeserializeMessage<T>(MessageProperties properties, Byte[] body) where T : class {
            var messageBody = _serializer.BytesToMessage<T>(body);

            BindHeaderValues(typeof(T), messageBody, properties.Headers);

            return new Message<T>(messageBody, properties);
        }

        public SerializedMessage SerializeMessage(IMessage message) {
            var messageBody = message.GetBody();
            var messageProperties = message.Properties;

            var boundProperties = ScanForBoundProperties(message.MessageType);
            if (boundProperties.Any()) {
                messageProperties.HeadersPresent = true;
                messageProperties.Headers =
                    boundProperties.ToDictionary(GetHeaderKey, property => GetHeaderValue(property, messageBody));
            }

            var typeName = _typeNameSerializer.Serialize(message.MessageType);
            messageProperties.Type = typeName;

            if (String.IsNullOrEmpty(messageProperties.CorrelationId))
                messageProperties.CorrelationId = _correlationIdGenerator.GetCorrelationId();

            var messageBytes = _serializer.MessageToBytes(messageBody);
            return new SerializedMessage(messageProperties, messageBytes);
        }

        private void BindHeaderValues(IReflect messageType, Object messageBody, IDictionary<String, Object> headers) {
            var boundProperties = ScanForBoundProperties(messageType);

            // There is a special case when only one message header is defined - the singular header value will be
            // accessible using the key "header_value" rather than the original header key...
            if (boundProperties.Count() == 1) {
                var boundProperty = boundProperties.Single();
                var headerValue = GetHeaderValue(headers, "header_value");
                BindHeaderValue(boundProperty, messageBody, headerValue);
            }
            if (boundProperties.Count() > 1) {
                foreach (var boundProperty in boundProperties) {
                    var headerKey = GetHeaderKey(boundProperty);
                    var headerValue = GetHeaderValue(headers, headerKey);
                    BindHeaderValue(boundProperty, messageBody, headerValue);
                }
            }
        }

        private static IList<PropertyInfo> ScanForBoundProperties(IReflect type) {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.CanRead && property.CanWrite)
                .Where(property => property.GetCustomAttribute<MessageHeaderAttribute>(inherit: false) != null)
                .ToList();
        }

        private static String GetHeaderKey(PropertyInfo boundProperty) {
            var messageHeaderAttribute = boundProperty.GetCustomAttribute<MessageHeaderAttribute>(inherit: false);

            return messageHeaderAttribute.Key;
        }

        private static Object GetHeaderValue(PropertyInfo boundProperty, Object message) {
            var typeConverter = TypeDescriptor.GetConverter(boundProperty.PropertyType);
            var propertyValue = boundProperty.GetValue(message);

            return typeConverter.ConvertToInvariantString(propertyValue);
        }

        private static String GetHeaderValue(IDictionary<String, Object> headers, String key) {
            var headerBytes = (byte[])headers[key];
            return Encoding.UTF8.GetString(headerBytes);
        }

        private static void BindHeaderValue(PropertyInfo boundProperty, Object message, String headerValue) {
            var typeConverter = TypeDescriptor.GetConverter(boundProperty.PropertyType);
            var propertyValue = typeConverter.ConvertFromInvariantString(headerValue);

            boundProperty.SetValue(message, propertyValue);
        }
    }
}