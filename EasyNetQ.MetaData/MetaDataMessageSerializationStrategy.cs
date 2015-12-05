namespace EasyNetQ.MetaData {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

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

        public IMessage DeserializeMessage(MessageProperties properties, byte[] body) {
            throw new NotImplementedException();
        }

        public IMessage<T> DeserializeMessage<T>(MessageProperties properties, byte[] body) where T : class {
            throw new NotImplementedException();
        }

        public SerializedMessage SerializeMessage(IMessage message) {
            var messageBody = message.GetBody();
            var messageProperties = message.Properties;

            var boundProperties = ScanForBoundProperties(message.MessageType);
            if (boundProperties.Any()) {
                messageProperties.HeadersPresent = true;
                messageProperties.Headers =
                    boundProperties.ToDictionary(GetHeaderKey, property => GetHeaderValue(property, messageBody));
            };

            var typeName = _typeNameSerializer.Serialize(message.MessageType);
            messageProperties.Type = typeName;

            if (String.IsNullOrEmpty(messageProperties.CorrelationId))
                messageProperties.CorrelationId = _correlationIdGenerator.GetCorrelationId();

            var messageBytes = _serializer.MessageToBytes(messageBody);
            return new SerializedMessage(messageProperties, messageBytes);
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

            return typeConverter.ConvertToString(propertyValue);
        }
    }
}